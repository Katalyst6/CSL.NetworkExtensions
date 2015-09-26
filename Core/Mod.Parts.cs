using System;
using System.Collections.Generic;
using System.Linq;
using CSL.ExtensionFramework.ModParts;
using ICities;
using NetworkExtensions.Install;

namespace NetworkExtensions
{
    public partial class Mod : IUserMod
    {
        private static IEnumerable<IModPart> s_parts;
        public static IEnumerable<IModPart> Parts
        {
            get
            {
                if (s_parts == null)
                {
                    var partType = typeof(IModPart);

                    s_parts = typeof(RoadsInstaller)
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(partType.IsAssignableFrom)
                        .Select(t =>
                        {
                            var part = (IModPart)Activator.CreateInstance(t);

                            if (part is IActivablePart)
                            {
                                var activable = (IActivablePart) part;

                                activable.IsEnabled = Options.Instance.IsPartEnabled(activable);
                            }
                            return part;
                        })
                        .ToArray();
                }

                return s_parts;
            }
        }

        public static IEnumerable<IActivablePart> ActivableParts
        {
            get
            {
                return Parts
                    .OfType<IActivablePart>()
                    .OrderBy(p => p.OptionsPriority)
                    .ToArray();
            }
        }

        public static IEnumerable<INetInfoBuilder> NetInfoBuilders
        {
            get
            {
                return ActivableParts
                    .Where(p => p.IsEnabled)
                    .OfType<INetInfoBuilder>()
                    .ToArray();
            }
        }

        public static IEnumerable<INetInfoModifier> NetInfoModifiers
        {
            get
            {
                return ActivableParts
                    .Where(p => p.IsEnabled)
                    .OfType<INetInfoModifier>()
                    .ToArray();
            }
        }

        public static IEnumerable<ICompatibilityPart> CompatibilityParts
        {
            get
            {
                return Parts
                    .OfType<ICompatibilityPart>()
                    .ToArray();
            }
        }
    }
}
