using Apttus.XAuthor.AICAdapter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.Core
{
    public class ABAICV2Adapter : ABAICAAdapter
    {
        public ABAICV2Adapter()
        {
            aicConnector = AICV2ServiceController.GetInstance;
            aicConnector.session = new AICV2RefreshSession();
        }
        public override bool Connect(IXAuthorCRMLogin login)
        {
            IXAuthorAICV2Login loginctl = login as IXAuthorAICV2Login;
            var TokenResponse = JsonConvert.DeserializeObject<AICV2TokenResponse>(loginctl.TokenResponse);
            bool bConnected = aicConnector.ConnectWithAICV2(TokenResponse, loginctl.TenantURL, loginctl.Proxy);

            if (bConnected)
                Constants.NAMESPACE_PREFIX = "Dummy_Namespace_Prefix Which is not used in aic";

            apttusUserInfo = getUserInfo();
            return true;
        }
    }
}
