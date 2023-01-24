namespace Terrasoft.Configuration.MMDMS.Constants
{
	public static class MMDMSConstantsCs
	{
		public static class URIs
		{
			#region GET
			
			public static readonly string GetRecipientsListRequestURI = "http://sms.mmdsmart.com/api/3/recipientList/{0}/listNumbers";
			public static readonly string GetRecipientsListsRequestURI = "http://sms.mmdsmart.com/api/3/RecipientList";
			public static readonly string GetSendersRequestURI = "http://sms.mmdsmart.com/api/3/sender";
			public static readonly string GetCampaignsRequestURI = "http://sms.mmdsmart.com/api/3/campaign";
			public static readonly string GetBroadcastsRequestURI = "http://sms.mmdsmart.com/api/3/Broadcast/List";
			public static readonly string GetSMSTemplatesRequestURI = "http://sms.mmdsmart.com/api/3/template";
			public static readonly string GetSMSTriggersRequestURI = "http://sms.mmdsmart.com/api/3/trigger";
			public static readonly string GetUTMLinksRequestURI = "http://sms.mmdsmart.com/api/3/link";
			public static readonly string GetUnsubscribeListsRequestURI = "http://sms.mmdsmart.com/api/3/unsubscribeList";
			public static readonly string GetUnsubscribeListMembersRequestURI = "http://sms.mmdsmart.com/api/3/unsubscribeList/{0}/members";
			
			#endregion

			#region POST

			public static readonly string CreateRecipientsListRequestURI = "http://sms.mmdsmart.com/api/3/recipientList/numbers";
			public static readonly string CreateSenderRequestURI = "http://sms.mmdsmart.com/api/3/sender";
			public static readonly string CreateCampaignRequestURI = "http://sms.mmdsmart.com/api/3/campaign";
			public static readonly string CreateBroadcastRequestURI = "http://sms.mmdsmart.com/api/3/Broadcast";
			public static readonly string CreateSMSTemplateRequestURI = "http://sms.mmdsmart.com/api/3/template";
			public static readonly string CreateSMSTriggerRequestURI = "http://sms.mmdsmart.com/api/3/trigger";
			public static readonly string CreateUTMLinkRequestURI = "http://sms.mmdsmart.com/api/3/link";
			public static readonly string CreateUnsubscribeListURI = "http://sms.mmdsmart.com/api/3/unsubscribeList";
			public static readonly string CreateNewMembersInUnsubscribeListURI = "http://sms.mmdsmart.com/api/3/unsubscribeList/{0}/members";

			#endregion

			#region PUT

			public static readonly string ModifyRecipientsListRequestURI = "http://sms.mmdsmart.com/api/3/recipientList/{0}/numbers";

			#endregion
		}
	}
}