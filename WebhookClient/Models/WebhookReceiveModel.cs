using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookClient.Models
{
    public class WebhookReceiveModel
    {
        public Dictionary<Guid, Guid> ProductKeys { get; set; } //main product, clone product

        public Guid SourceTeamId { get; set; }

        public Guid TargetTeamId { get; set; }
    }
}
