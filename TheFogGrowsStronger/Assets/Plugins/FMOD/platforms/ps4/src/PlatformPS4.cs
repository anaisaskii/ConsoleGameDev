using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_PS4 && !UNITY_EDITOR
namespace FMOD
{
    public partial class VERSION
    {
        public const string dll = "libfmod" + dllSuffix;
    }
}

namespace FMOD.Studio
{
    public partial class STUDIO_VERSION
    {
        public const string dll = "libfmodstudio" + dllSuffix;
    }
}
#endif

namespace FMODUnity
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class PlatformPS4 : Platform
    {
        static PlatformPS4()
        {
            Settings.AddPlatformTemplate<PlatformPS4>("2cb23fe3bedf0034f99bb5eba23712a5");
        }

        internal override string DisplayName { get { return "PS4"; } }
        internal override void DeclareRuntimePlatforms(Settings settings)
        {
            settings.DeclareRuntimePlatform(RuntimePlatform.PS4, this);
        }

#if UNITY_EDITOR
        internal override IEnumerable<BuildTarget> GetBuildTargets()
        {
            yield return BuildTarget.PS4;
        }

        internal override Legacy.Platform LegacyIdentifier { get { return Legacy.Platform.PS4; } }

        protected override BinaryAssetFolderInfo GetBinaryAssetFolder(BuildTarget buildTarget)
        {
            return new BinaryAssetFolderInfo("ps4", "Plugins/PS4");
        }

        protected override IEnumerable<FileRecord> GetBinaryFiles(BuildTarget buildTarget, bool allVariants, string suffix)
        {
            yield return new FileRecord(string.Format("libfmod{0}.prx", suffix));
            yield return new FileRecord(string.Format("libfmodstudio{0}.prx", suffix));
        }

        protected override IEnumerable<FileRecord> GetOptionalBinaryFiles(BuildTarget buildTarget, bool allVariants)
        {
            yield return new FileRecord("libresonanceaudio.prx");
        }

        protected override IEnumerable<FileRecord> GetSourceFiles()
        {
            yield return new FileRecord("fmod_ps4.cs");
        }

        protected override IEnumerable<string> GetObsoleteFiles()
        {
            // resonanceaudio.prx
            yield return "lib/ps4/resonanceaudio.prx";
            yield return "lib/ps4/libresonanceaudio.prx";
        }
#endif

        internal override string GetPluginPath(string pluginName)
        {
            return string.Format("{0}/lib{1}.prx", GetPluginBasePath(), pluginName);
        }
#if UNITY_EDITOR
        internal override OutputType[] ValidOutputTypes
        {
            get
            {
                return sValidOutputTypes;
            }
        }

        private static OutputType[] sValidOutputTypes = {
           new OutputType() { displayName = "Audio Out", outputType = FMOD.OUTPUTTYPE.AUDIOOUT },
           new OutputType() { displayName = "Audio 3D", outputType = FMOD.OUTPUTTYPE.AUDIO3D },
        };

        internal override int CoreCount { get { return 7; } }
#endif

        internal override List<ThreadAffinityGroup> DefaultThreadAffinities { get { return StaticThreadAffinities; } }

        private static List<ThreadAffinityGroup> StaticThreadAffinities = new List<ThreadAffinityGroup>() {
            new ThreadAffinityGroup(ThreadAffinity.Core2, ThreadType.Mixer, ThreadType.Feeder, ThreadType.Record),
            new ThreadAffinityGroup(ThreadAffinity.Core4,
                ThreadType.Studio_Update, ThreadType.Studio_Load_Bank, ThreadType.Studio_Load_Sample),
        };

        internal override List<CodecChannelCount> DefaultCodecChannels { get { return staticCodecChannels; } }

        private static List<CodecChannelCount> staticCodecChannels = new List<CodecChannelCount>()
        {
            new CodecChannelCount { format = CodecType.AT9, channels = 32 },
            new CodecChannelCount { format = CodecType.FADPCM, channels = 0 },
            new CodecChannelCount { format = CodecType.Vorbis, channels = 0 },
        };
    }
}
