using System;
using System.Collections.Generic;

namespace WebhookRegistrator
{
    internal class MenuGroup
    {
        public List<MenuGroupWriteModel> Groups { get; set; }
        public string Language { get; set; }
    }

    public class MenuGroupWriteModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}