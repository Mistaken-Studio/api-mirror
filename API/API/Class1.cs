using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mistaken.API
{
    public abstract class AutoUpdatablePlugin<TConfig> : Plugin<TConfig> where TConfig : IAutoUpdatableConfig, new()
    {

    }

    public interface IAutoUpdatableConfig : IConfig
    {

    }
}
