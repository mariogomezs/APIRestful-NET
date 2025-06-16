using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using Spark.Brim.Dto;
using SPark_WebManagement.Core.Business.Brim;
using SPark_WebManagement.Core.Business.DocumentFormat;
using SPark_WebManagement.Core.Business.Mail;
using SPark_WebManagement.Core.Business.Mapper;
using SPark_WebManagement.Core.Business.Sipay;
using SPark_WebManagement.Core.Business.WebManagement.WebManagementManual;
using SPark_WebManagement.Core.Commons.Codes;
using SPark_WebManagement.Core.Commons.Exceptions;
using SPark_WebManagement.Core.Commons.Extensions;
using SPark_WebManagement.Core.Commons.Log;
using SPark_WebManagement.Core.Commons.Token;
using SPark_WebManagement.Core.Entities;
using SPark_WebManagement.Core.Entities.Brim;
using SPark_WebManagement.Core.Entities.Mail;
using SPark_WebManagement.Core.Entities.Parking;
using SPark_WebManagement.Core.Entities.Pay;
using SPark_WebManagement.Core.Entities.Sipay;
using SPark_WebManagement.Core.Entities.Stay;
using SPark_WebManagement.Core.Entities.Stay.Debt;
using SPark_WebManagement.Core.Entities.Stay.ExcelModels;
using SPark_WebManagement.Core.Entities.Stay.Refund;
using SPark_WebManagement.Core.Entities.Stay.StayFiltersDto;
using SPark_WebManagement.Core.Entities.Tcr;
using SPark_WebManagement.Core.Infrastructure.Common;
using SPark_WebManagement.Core.Infrastructure.Parking;
using SPark_WebManagement.Core.Infrastructure.Pay;
using SPark_WebManagement.Core.Infrastructure.Stay;
using SPark_WebManagement.Core.Infrastructure.Tcr;
using SPark_WebManagement.Core.Infrastructure.User;
using SPark_WebManagement.Core.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SPark_WebManagement.Core.Business.WebManagement.WebManagementStays
{
    public class WebManagementStaysService : IWebManagementStaysService
    {
        #region Private Properties
        private readonly ILogManager logManager;
        private readonly IStayInfra stayRepository;
        private readonly IDocumentFormat documentFormat;
        private readonly IPayRepository payRepository;
        private readonly IWebManagementManualService manualService;
        private readonly IParkingRepository parkingRepository;
        private readonly IBrimService brimService;
        private readonly IUserInfra userRepository;
        private readonly ISipayService sipayService;
        private readonly IMailService mailService;
        private readonly ICommonRepository commonRepository;
        private readonly IConfiguration configuration;
        private readonly ITcrRepository tcrRepository;

        private readonly string className = "SiPark.Core.Business.WebManagement.WebManagementStays.WebManagementStaysService";

        #endregion

        #region Constructor
        public WebManagementStaysService(ILogManager logManager,
            IStayInfra stayRepository,
            IDocumentFormat documentFormat,
            IPayRepository payRepository,
            IWebManagementManualService manualService,
            IParkingRepository parkingRepository,
            IBrimService brimService,
            IUserInfra userRepository,
            ISipayService sipayService,
            IMailService mailService,
            ICommonRepository commonRepository,
            IConfiguration configuration,
            ITcrRepository tcrRepository)
        {
            this.logManager = logManager;
            this.stayRepository = stayRepository;
            this.documentFormat = documentFormat;
            this.payRepository = payRepository;
            this.manualService = manualService;
            this.parkingRepository = parkingRepository;
            this.brimService = brimService;
            this.userRepository = userRepository;
            this.sipayService = sipayService;
            this.mailService = mailService;
            this.commonRepository = commonRepository;
            this.configuration = configuration;
            this.tcrRepository = tcrRepository;
        }
        #endregion

        #region Public Methods
        #region Stay operations
        public async Task<GetStaysResponse> GetStays(StayFilterRequest filters, StayOrderingFilterRequest orderingFilters, bool includeTmp)
        {
            GetStaysModel staysData = await this.stayRepository.GetStays(filters, includeTmp);

            if (staysData == null || staysData.StaysData.Count == 0)
            {
                return new GetStaysResponse()
                {
                    ActualCount = 0,
                    TotalCount = 0,
                    Result = OperationResultCodes.Error,
                    StayData = null
                };
            }

            var limitedStays = staysData.StaysData.OrderByDescending(stay => stay.StaysId).Take(2000).ToList();

            var orderedStays = OrderStays(limitedStays, orderingFilters);

            return new GetStaysResponse()
            {
                Result = OperationResultCodes.Correct,
                TotalCount = staysData.TotalDataCount,
                ActualCount = limitedStays.Count,
                StayData = orderedStays
            };
        }

        public async Task<GetStayResponseDto> GetStay(long stayId)
        {
            this.logManager.Debug(this.className, "StayBusiness: GET STAY OPERATION");
            var paysData = await this.payRepository.GetPaysByStayId(stayId);
            var stayData = await this.stayRepository.GetStayData(stayId);
            if (stayData == null)
            {
                return new GetStayResponseDto()
                {
                    Result = (int)OperationResultCodes.Error,
                    Message = "Unable to find stay.",
                    Data = null
                };
            }

            var device = await parkingRepository.GetDeviceByTerminalNumber(stayData.ParkingId, stayData.EntryWayId.Value);
            stayData.DeviceDescription = device == null ? string.Empty : device.Description;

            stayData.Pays = StayMapper.PaysEntityListToPaysDataList(paysData, stayData.ProductCategoryId);

            //Recuperamos los movimentos
            List<MovementData> movements = await stayRepository.GetMovementsByIdStay(stayId);
            //Recuperamos los comments (operaciones manuales)
            List<CommentEntity> comments = await stayRepository.GetCommentsByIdStay(stayId);

            stayData.Movements = new();
            stayData.Movements.AddRange(movements);
            stayData.Movements.AddRange(StayMapper.MapCommentEntityToMovementData(comments, device));

            List<long> paysIds = paysData.Select(x => x.PayId).Distinct().ToList();

            if (paysIds.Count > 0)
            {
                //Recuperamos el total pagado por el cliente
                stayData.TotalPayment = await this.payRepository.GetTotalPaymentByIdPay(paysIds);
            }

            return new GetStayResponseDto()
            {
                Result = (int)OperationResultCodes.Correct,
                Message = $"StayID: {stayData.StaysId} - ParkingNumber: {stayData.ParkingId} FOUND",
                Data = stayData
            };
        }