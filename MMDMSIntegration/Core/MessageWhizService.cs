using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Terrasoft.Configuration.MMDMS.Constants;
using Terrasoft.Configuration.MMDMS.Core;
using Terrasoft.Configuration.MMDMS.Models.RequestsModels;
using Terrasoft.Configuration.MMDMS.Models.ResponseModels;
using Terrasoft.Core;
using Terrasoft.Core.Entities;
using Terrasoft.Web.Common;

namespace Terrasoft.Configuration.MMDMS
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MessageWhizService : BaseService
    {
        private SystemUserConnection _systemUserConnection;
        private SystemUserConnection SystemUserConnection
        {
            get
            {
                return _systemUserConnection ?? (_systemUserConnection = (SystemUserConnection)AppConnection.SystemUserConnection);
            }
        }

        private string _messageWhizAPIKey;
        private string MessageWhizAPIKey
        {
            get
            {
                { return _messageWhizAPIKey ?? (_messageWhizAPIKey = Terrasoft.Core.Configuration.SysSettings.GetValue(SystemUserConnection, "MessageWhizAPIKey").ToString()); }
            }
        }

        private MessageWhizRouter _messageWhizRouter;

        public MessageWhizService()
        {
            _messageWhizRouter = new MessageWhizRouter(MessageWhizAPIKey);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
      ResponseFormat = WebMessageFormat.Json)]
        public void GetUnsubscribeLists()
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.GetUnsubscribeListsRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<GetUnsubscribeListsDataModel>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            result.result.ForEach(unsubscribeList =>
            {
                var systemUnsubscribeList = new UnsibscribeList(SystemUserConnection);
                if (!systemUnsubscribeList.FetchFromDB("UnsibscribeListId", unsubscribeList.id))
                {
                    systemUnsubscribeList.SetDefColumnValues();
                    systemUnsubscribeList.SetColumnValue("UnsibscribeListId", unsubscribeList.id);
                }

                systemUnsubscribeList.SetColumnValue("Title", unsubscribeList.name);
                systemUnsubscribeList.SetColumnValue("IsEnabled", unsubscribeList.enabled == 1);
                systemUnsubscribeList.SetColumnValue("UnsubscribersCount", unsubscribeList.unsubscribers_amount);

                systemUnsubscribeList.Save();
            });
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void GetUnsubscribeListMembers(List<Guid> unsubscriberIds, int unsubscribeListIdInt, Guid unsubscribeListId)
        {
            _messageWhizRouter.ChangeRoute(string.Format(MMDMSConstantsCs.URIs.GetUnsubscribeListMembersRequestURI, unsubscribeListIdInt));

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<GetUnsubscribeListMembersResponseDataModel>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            var unsubscribeList = new UnsibscribeList(SystemUserConnection);
            if (unsubscribeList.FetchFromDB("Id", unsubscribeListId))
            {
                var existsContactsCollection = GetSpecifiedEntityCollection("Unsubscriber", unsubscriberIds)
                    .Select(unsubscriber => unsubscriber.GetTypedColumnValue<Guid>("ContactId")).ToList();

                var existsContactsPhonesCollection = GetSpecifiedEntityCollection("Contact", existsContactsCollection)
                            .Select(contact => contact.GetTypedColumnValue<string>("MobilePhone")).ToList();

                result.result.ForEach(phoneNumber =>
                {
                    if (existsContactsPhonesCollection.FirstOrDefault(existPhone => existPhone == phoneNumber.phone_number) == null)
                    {
                        var contactWithSpecifiedPhone = new Contact(SystemUserConnection);
                        if (contactWithSpecifiedPhone.FetchFromDB("MobilePhone", phoneNumber.phone_number))
                        {
                            var newUnsubscriber = new Unsubscriber(SystemUserConnection);

                            newUnsubscriber.SetDefColumnValues();
                            newUnsubscriber.SetColumnValue("UnsibscribeListId", unsubscribeListId);
                            newUnsubscriber.SetColumnValue("ContactId", contactWithSpecifiedPhone.PrimaryColumnValue);

                            newUnsubscriber.Save();
                        }
                    }
                });
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
      ResponseFormat = WebMessageFormat.Json)]
        public void GetUTMLinks()
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.GetUTMLinksRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<GetUTMLinksResponseDataModel>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            result.result.ForEach(link =>
            {
                var systemLink = new UTMLink(SystemUserConnection);
                if (!systemLink.FetchFromDB("UTMLinkId", link.id))
                {
                    systemLink.SetDefColumnValues();
                    systemLink.SetColumnValue("UTMLinkId", link.id);
                }

                systemLink.SetColumnValue("IsEnabled", link.enabled);
                systemLink.SetColumnValue("Link", link.url);
                systemLink.SetColumnValue("Title", link.name);

                systemLink.Save();
            });
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
       ResponseFormat = WebMessageFormat.Json)]
        public void GetSMSTriggers()
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.GetSMSTriggersRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<GetSMSTriggersResponseDataModel>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            result.result.ForEach(trigger =>
            {
                var systemTrigger = new SMSTrigger(SystemUserConnection);
                if (!systemTrigger.FetchFromDB("SMSTriggerId", trigger.id))
                {
                    systemTrigger.SetDefColumnValues();
                    systemTrigger.SetColumnValue("SMSTriggerId", trigger.id);
                }

                systemTrigger.SetColumnValue("IsEnabled", trigger.enabled == 1);
                systemTrigger.SetColumnValue("ShortURL", trigger.short_url);
                systemTrigger.SetColumnValue("SMSTriggerUId", trigger.uuid);
                systemTrigger.SetColumnValue("Title", trigger.name);

                systemTrigger.Save();
            });
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void GetSMSTemplates()
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.GetSMSTemplatesRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<GetSMSTemplateResponseDataModel>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            result.result.ForEach(smsTemplate =>
            {
                var systemSMSTemplate = new SMSTemplate(SystemUserConnection);
                if (!systemSMSTemplate.FetchFromDB("SMSTemplateId", smsTemplate.id))
                {
                    systemSMSTemplate.SetDefColumnValues();
                    systemSMSTemplate.SetColumnValue("SMSTemplateId", smsTemplate.id);
                }

                systemSMSTemplate.SetColumnValue("IsEnabled", smsTemplate.enabled);
                systemSMSTemplate.SetColumnValue("Body", smsTemplate.body);
                systemSMSTemplate.SetColumnValue("Title", smsTemplate.name);

                systemSMSTemplate.Save();
            });
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void GetBroadcasts()
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.GetBroadcastsRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<GetBroadcastsResponseDataModel>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            result.result.ForEach(broadcast =>
            {
                var systemBroadcast = new Broadcast(SystemUserConnection);
                if (!systemBroadcast.FetchFromDB("BroadcastId", broadcast.id))
                {
                    systemBroadcast.SetDefColumnValues();
                    systemBroadcast.SetColumnValue("BroadcastId", broadcast.id);
                    systemBroadcast.SetColumnValue("MessageBody", broadcast.message_body);

                    var systemBroadcastType = new BroadcastType(SystemUserConnection);
                    if (systemBroadcastType.FetchFromDB("Code", broadcast.type))
                        systemBroadcast.SetColumnValue("BroadcastTypeId", systemBroadcastType.PrimaryColumnValue);
                }

                systemBroadcast.SetColumnValue("Name", broadcast.name);
                systemBroadcast.SetColumnValue("CompanyName", broadcast.company_name);
                systemBroadcast.SetColumnValue("IsSendNow", broadcast.send_now);
                systemBroadcast.SetColumnValue("CreationDate", broadcast.create_date);
                systemBroadcast.SetColumnValue("SendDate", broadcast.send_date);
                systemBroadcast.SetColumnValue("EstimatedPrice", broadcast.estimated_price);
                systemBroadcast.SetColumnValue("RealPrice", broadcast.real_price);
                systemBroadcast.SetColumnValue("IsEnabled", broadcast.enabled == 1);
                systemBroadcast.SetColumnValue("FailedCount", broadcast.dlr.failed_count);
                systemBroadcast.SetColumnValue("SentCount", broadcast.dlr.sent_count);
                systemBroadcast.SetColumnValue("DeliveredCount", broadcast.dlr.delivered_count);
                systemBroadcast.SetColumnValue("UndeliveredCount", broadcast.dlr.undelivered_count);
                systemBroadcast.SetColumnValue("RejectedCount", broadcast.dlr.rejected_count);
                systemBroadcast.SetColumnValue("ExpiredCount", broadcast.dlr.expired_count);

                systemBroadcast.Save();
            });
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void GetCampaigns()
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.GetCampaignsRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<GetCampaignsResponseDataModel>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            result.result.ForEach(campaign =>
            {
                var systemCampaign = new Campaign(SystemUserConnection);
                if (!systemCampaign.FetchFromDB("CampaignId", campaign.id))
                {
                    systemCampaign.SetDefColumnValues();
                    systemCampaign.SetColumnValue("CampaignId", campaign.id);
                    systemCampaign.SetColumnValue("Name", campaign.name);
                }

                systemCampaign.SetColumnValue("IsEnabled", campaign.enabled);
                systemCampaign.SetColumnValue("EndDate", campaign.end_date);
                systemCampaign.SetColumnValue("StartDate", campaign.start_date);

                systemCampaign.Save();
            });
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void GetSenders()
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.GetSendersRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<SenderResponseDataModel>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            result.result.ForEach(sender =>
            {
                var systemSender = new Sender(SystemUserConnection);
                if (!systemSender.FetchFromDB("SenderId", sender.id))
                {
                    systemSender.SetDefColumnValues();
                    systemSender.SetColumnValue("SenderId", sender.id);
                }

                systemSender.SetColumnValue("IsEnabled", sender.enabled);
                systemSender.SetColumnValue("SenderName", sender.name);

                systemSender.Save();
            });
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void GetRecipientsList(List<Guid> recepientsIds, int recepientListIdInt, Guid recipientListId)
        {
            _messageWhizRouter.ChangeRoute(string.Format(MMDMSConstantsCs.URIs.GetRecipientsListRequestURI, recepientListIdInt));

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<string>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            var recipientList = new RecipientList(SystemUserConnection);
            if (recipientList.FetchFromDB("Id", recipientListId))
            {
                var existsContactsCollection = GetSpecifiedEntityCollection("Recipient", recepientsIds)
                    .Select(recipient => recipient.GetTypedColumnValue<Guid>("ContactId")).ToList();

                var existsContactsPhonesCollection = GetSpecifiedEntityCollection("Contact", existsContactsCollection)
                            .Select(contact => contact.GetTypedColumnValue<string>("MobilePhone")).ToList();

                result.result.ForEach(phoneNumber =>
                {
                    if (existsContactsPhonesCollection.FirstOrDefault(existPhone => existPhone == phoneNumber) == null)
                    {
                        var contactWithSpecifiedPhone = new Contact(SystemUserConnection);
                        if (contactWithSpecifiedPhone.FetchFromDB("MobilePhone", phoneNumber))
                        {
                            var newRecipient = new Recipient(SystemUserConnection);

                            newRecipient.SetDefColumnValues();
                            newRecipient.SetColumnValue("RecipientListId", recipientListId);
                            newRecipient.SetColumnValue("ContactId", contactWithSpecifiedPhone.PrimaryColumnValue);

                            newRecipient.Save();
                        }
                    }
                });
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void GetRecipientsLists()
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.GetRecipientsListsRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<List<GetRecipientsListsResponseDataModel>>>(
                _messageWhizRouter
                .GetRequestResult("GET"));

            result.result.ForEach(recipientsList =>
            {
                var systemRecipientsList = new RecipientList(SystemUserConnection);
                if (!systemRecipientsList.FetchFromDB("RecipientListId", recipientsList.id))
                {
                    systemRecipientsList.SetDefColumnValues();
                    systemRecipientsList.SetColumnValue("RecipientListId", recipientsList.id);
                    systemRecipientsList.SetColumnValue("RecipientListName", recipientsList.name);
                }

                systemRecipientsList.SetColumnValue("PhoneNumbersCount", recipientsList.count);
                systemRecipientsList.SetColumnValue("IsEnabled", recipientsList.enabled == 1);

                systemRecipientsList.Save();

                GetRecipientsList(
                    GetSpecifiedEntityCollection(
                        "Recipient",
                        null,
                        "RecipientList",
                        systemRecipientsList.PrimaryColumnValue.ToString()).Select(
                        recipient => recipient.GetTypedColumnValue<Guid>("Id")).ToList(),
                    recipientsList.id,
                    systemRecipientsList.PrimaryColumnValue);
            });
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
     ResponseFormat = WebMessageFormat.Json)]
        public void AddMemberToUnsubscribeList(List<Guid> contactIds, int unsubscribeListIdInt)
        {
            _messageWhizRouter.ChangeRoute(string.Format(MMDMSConstantsCs.URIs.CreateNewMembersInUnsubscribeListURI, unsubscribeListIdInt));

            _messageWhizRouter
                .GetRequestResult(
                    "POST",
                    JsonConvert.SerializeObject(new PublishModifiedUnsubscribeListRequestDataModel()
                    {
                        numbers = GetSpecifiedEntityCollection("Contact", contactIds)
                            .Select(contact => contact.GetTypedColumnValue<string>("MobilePhone"))
                            .ToList()
                    }));
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
      ResponseFormat = WebMessageFormat.Json)]
        public void PublishUnsubscribeList(List<Guid> contactIds, string unsubscribeListName, Guid unsubscribeListId)
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.CreateUnsubscribeListURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<PublishUnsubscribeListResponseDataModel>>(_messageWhizRouter
                .GetRequestResult(
                    "POST",
                    JsonConvert.SerializeObject(new PublishUnsubscribeListRequestDataModel()
                    {
                        name = unsubscribeListName,
                        numbers = GetSpecifiedEntityCollection("Contact", contactIds)
                            .Select(contact => contact.GetTypedColumnValue<string>("MobilePhone"))
                            .ToList()
                    })));

            var systemUnsubscribeList = new UnsibscribeList(SystemUserConnection);
            if (!systemUnsubscribeList.FetchFromDB("UnsibscribeListId", result.result.id))
            {
                systemUnsubscribeList.SetDefColumnValues();
                systemUnsubscribeList.SetColumnValue("UnsibscribeListId", result.result.id);
                systemUnsubscribeList.SetColumnValue("Title", result.result.name);
                systemUnsubscribeList.SetColumnValue("IsEnabled", result.result.enabled == 1);
                systemUnsubscribeList.SetColumnValue("UnsubscribersCount", result.result.unsubscribers_amount);

                systemUnsubscribeList.Save();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
       ResponseFormat = WebMessageFormat.Json)]
        public void PublishUTMLink(
           string linkName,
           string linkURL,
           Guid linkId)
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.CreateUTMLinkRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<PublishUTMLinkResponseDataModel>>(_messageWhizRouter
                .GetRequestResult(
                    "POST",
                    JsonConvert.SerializeObject(new PublishUTMLinkRequestDataModel()
                    {
                        name = linkName,
                        url = linkURL
                    })));

            var systemLink = new UTMLink(SystemUserConnection);
            if (systemLink.FetchFromDB("Id", linkId))
            {
                systemLink.SetColumnValue("UTMLinkId", result.result.id);

                systemLink.SetColumnValue("IsEnabled", result.result.enabled);
                systemLink.SetColumnValue("Link", result.result.url);
                systemLink.SetColumnValue("Title", result.result.name);

                systemLink.Save();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
       ResponseFormat = WebMessageFormat.Json)]
        public void PublishSMSTrigger(
           string triggerName,
           Guid triggerId)
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.CreateSMSTriggerRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<PublishSMSTriggerResponseDataModel>>(_messageWhizRouter
                .GetRequestResult(
                    "POST",
                    JsonConvert.SerializeObject(new PublishRequestDataModel()
                    {
                        name = triggerName
                    })));

            var systemTrigger = new SMSTrigger(SystemUserConnection);
            if (systemTrigger.FetchFromDB("Id", triggerId))
            {
                systemTrigger.SetColumnValue("SMSTriggerId", result.result.id);
                systemTrigger.SetColumnValue("IsEnabled", result.result.enabled == 1);
                systemTrigger.SetColumnValue("ShortURL", result.result.short_url);
                systemTrigger.SetColumnValue("SMSTriggerUId", result.result.uuid);

                systemTrigger.Save();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void PublishSMSTemplate(
            string smsTemplateName,
            string smsTemplateBody,
            Guid smsTemplateId)
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.CreateSMSTemplateRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<PublishSMSTemplateResponseDataModel>>(_messageWhizRouter
                .GetRequestResult(
                    "POST",
                    JsonConvert.SerializeObject(new PublishSMSTemplateRequestDataModel()
                    {
                        name = smsTemplateName,
                        body = smsTemplateBody
                    })));

            var systemSMSTemplate = new SMSTemplate(SystemUserConnection);
            if (systemSMSTemplate.FetchFromDB("Id", smsTemplateId))
            {
                systemSMSTemplate.SetColumnValue("SMSTemplateId", result.result.id);
                systemSMSTemplate.SetColumnValue("IsEnabled", result.result.enabled);

                systemSMSTemplate.Save();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void PublishBroadcast(
            string broadcastName,
            int campaignId,
            int broadcastType,
            string senderIds,
            string recipientListIds,
            string messageBody,
            string unsubscribeListIds,
            int utcOffset,
            string sendDate,
            int templateId,
            int delay,
            int triggerId,
            Guid broadcastId)
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.CreateBroadcastRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<PublishBroadcastResponseDataModel>>(_messageWhizRouter
                .GetRequestResult(
                    "POST",
                    JsonConvert.SerializeObject(new PublishBroadcastRequestDataModel()
                    {
                        name = broadcastName,
                        campaign_id = campaignId,
                        broadcast_type = broadcastType,
                        sender_ids = senderIds,
                        recipient_list_ids = recipientListIds,
                        message_body = messageBody,
                        template_id = templateId,
                        delay = delay,
                        send_date = sendDate,
                        trigger_id = triggerId,
                        unsubscriber_list_ids = unsubscribeListIds,
                        utc_offset = utcOffset,
                    })));
            var broadcast = new Broadcast(SystemUserConnection);
            if (broadcast.FetchFromDB("Id", broadcastId))
            {
                broadcast.SetColumnValue("BroadcastId", result.result.broadcastID);

                broadcast.Save();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void PublishCampaign(string campaignName, DateTime? startDate, DateTime? endDate, Guid campaignId)
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.CreateCampaignRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<PublishCampaignResponseDataModel>>(_messageWhizRouter
                .GetRequestResult(
                    "POST",
                    JsonConvert.SerializeObject(new PublishCampaignRequestDataModel(campaignName, startDate, endDate))));
            var campaign = new Campaign(SystemUserConnection);
            if (campaign.FetchFromDB("Id", campaignId))
            {
                campaign.SetColumnValue("CampaignId", result.result.id);
                campaign.SetColumnValue("IsEnabled", result.result.enabled);

                campaign.Save();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void PublishSender(string senderName, Guid senderId)
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.CreateSenderRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<SenderResponseDataModel>>(_messageWhizRouter
                .GetRequestResult(
                    "POST",
                    JsonConvert.SerializeObject(new PublishRequestDataModel()
                    {
                        name = senderName
                    })));

            var systemSender = new Sender(SystemUserConnection);
            if (systemSender.FetchFromDB("Id", senderId))
            {
                systemSender.SetColumnValue("IsEnabled", result.result.enabled);
                systemSender.SetColumnValue("SenderId", result.result.id);

                systemSender.Save();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void PublishRecipientsList(List<Guid> contactIds, string name, Guid recipientListId)
        {
            _messageWhizRouter.ChangeRoute(MMDMSConstantsCs.URIs.CreateRecipientsListRequestURI);

            var result = JsonConvert.DeserializeObject<ResponseDataModel<PublishRecipientsListResponseDataModel>>(_messageWhizRouter
                .GetRequestResult(
                    "POST",
                    JsonConvert.SerializeObject(new PublishRecipientsListRequestDataModel()
                    {
                        name = name,
                        numbers = GetSpecifiedEntityCollection("Contact", contactIds)
                            .Select(contact => contact.GetTypedColumnValue<string>("MobilePhone"))
                            .ToList()
                    })));
            var recipientList = new RecipientList(SystemUserConnection);
            if (recipientList.FetchFromDB("Id", recipientListId))
            {
                recipientList.SetColumnValue("PhoneNumbersCount", result.result.inserted);
                recipientList.SetColumnValue("RecipientListId", result.result.recipient_list_id);

                recipientList.Save();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json)]
        public void PublishModifiedRecipientsList(List<Guid> contactIds, string name, int recepientListIdInt)
        {
            _messageWhizRouter.ChangeRoute(string.Format(MMDMSConstantsCs.URIs.ModifyRecipientsListRequestURI, recepientListIdInt));

            var result = JsonConvert.DeserializeObject<PublishRecipientsListResponseDataModel>(_messageWhizRouter
                .GetRequestResult(
                    "PUT",
                    JsonConvert.SerializeObject(new PublishRecipientsListRequestDataModel()
                    {
                        name = name,
                        numbers = GetSpecifiedEntityCollection("Contact", contactIds)
                            .Select(contact => contact.GetTypedColumnValue<string>("MobilePhone"))
                            .ToList()
                    })));
        }

        private EntityCollection GetSpecifiedEntityCollection(
            string entitySchemaName,
            List<Guid> ids = null,
            string filterColumnName = "",
            string filterValue = "",
            FilterComparisonType filterComparisonType = FilterComparisonType.Equal)
        {
            var entityESQ = new EntitySchemaQuery(SystemUserConnection.EntitySchemaManager, entitySchemaName);
            entityESQ.AddAllSchemaColumns();

            if (ids != null)
            {
                if (ids.Count > 0)
                {
                    var filterCollection = new EntitySchemaQueryFilterCollection(entityESQ, Common.LogicalOperationStrict.Or);

                    ids.ForEach(id =>
                    {
                        filterCollection.Add(entityESQ.CreateFilterWithParameters(
                            FilterComparisonType.Equal,
                            "Id",
                            id));
                    });

                    entityESQ.Filters.Add(filterCollection);
                }
                else
                    entityESQ.Filters.Add(entityESQ.CreateIsNullFilter("Id"));
            }

            if (filterColumnName.Length > 0)
            {
                entityESQ.Filters.Add(entityESQ.CreateFilterWithParameters(
                    filterComparisonType,
                    filterColumnName,
                    filterValue));
            }

            return entityESQ.GetEntityCollection(SystemUserConnection);
        }
    }
}