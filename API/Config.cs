using System.ComponentModel;

namespace Mistaken.API
{
    internal sealed class Config
    {
        [Description("If true then debug will be displayed")]
        public bool Debug { get; set; } = false;
    }
}
