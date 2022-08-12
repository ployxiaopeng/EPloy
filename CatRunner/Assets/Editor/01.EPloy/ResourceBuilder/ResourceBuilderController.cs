using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;
using EPloy.Game.Res;
using EPloy.Game;

namespace EPloy.Editor.ResourceTools
{
    public sealed partial class ResourceBuilderController
    {

        private static readonly int AssetsStringLength = "Assets".Length;

        private readonly string m_ConfigurationPath;
        private readonly ResourceCollection m_ResourceCollection;
        private readonly ResourceAnalyzerController m_ResourceAnalyzerController;
        private readonly SortedDictionary<string, ResourceData> m_ResourceDatas;
        private readonly BuildReport m_BuildReport;

        public ResourceBuilderController()
        {
            m_ConfigurationPath = UtilPath.GetRegularPath(Path.Combine(Application.dataPath, EPloyEditorPath.ResBuilder));

            m_ResourceCollection = new ResourceCollection();
            m_ResourceCollection.OnLoadingResource += delegate (int index, int count)
            {
                if (OnLoadingResource != null)
                {
                    OnLoadingResource(index, count);
                }
            };

            m_ResourceCollection.OnLoadingAsset += delegate (int index, int count)
            {
                if (OnLoadingAsset != null)
                {
                    OnLoadingAsset(index, count);
                }
            };

            m_ResourceCollection.OnLoadCompleted += delegate ()
            {
                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }
            };

            m_ResourceAnalyzerController = new ResourceAnalyzerController(m_ResourceCollection);

            m_ResourceAnalyzerController.OnAnalyzingAsset += delegate (int index, int count)
            {
                if (OnAnalyzingAsset != null)
                {
                    OnAnalyzingAsset(index, count);
                }
            };

            m_ResourceAnalyzerController.OnAnalyzeCompleted += delegate ()
            {
                if (OnAnalyzeCompleted != null)
                {
                    OnAnalyzeCompleted();
                }
            };

            m_ResourceDatas = new SortedDictionary<string, ResourceData>(StringComparer.Ordinal);
            m_BuildReport = new BuildReport();

            Platforms = Platform.Undefined;
            AssetBundleZip = AssetBundleZipType.LZ4;
            ZipHelperTypeName = string.Empty;
            AdditionalZipSelected = false;
            ForceRebuildAssetBundleSelected = false;
            BuildEventHandlerTypeName = string.Empty;
            OutputDirectory = string.Empty;
            OutputPackageSelected = OutputFullSelected = true;
        }

        public string ProductName
        {
            get
            {
                return PlayerSettings.productName;
            }
        }

        public string CompanyName
        {
            get
            {
                return PlayerSettings.companyName;
            }
        }

        public string GameIdentifier
        {
            get
            {
                return PlayerSettings.applicationIdentifier;
            }
        }

        public string Version
        {
            get
            {
                return "1.5测试版";
            }
        }

        public string UnityVersion
        {
            get
            {
                return Application.unityVersion;
            }
        }

        public string ApplicableGameVersion
        {
            get
            {
                return Application.version;
            }
        }

        public int InternalResourceVersion
        {
            get;
            set;
        }

        public Platform Platforms
        {
            get;
            set;
        }

        public AssetBundleZipType AssetBundleZip
        {
            get;
            set;
        }

        public string ZipHelperTypeName
        {
            get;
            set;
        }

        public bool AdditionalZipSelected
        {
            get;
            set;
        }

        public bool ForceRebuildAssetBundleSelected
        {
            get;
            set;
        }

        public string BuildEventHandlerTypeName
        {
            get;
            set;
        }

        public string OutputDirectory
        {
            get;
            set;
        }

        public bool OutputPackageSelected
        {
            get;
            set;
        }

        public bool OutputFullSelected
        {
            get;
            set;
        }

        public bool IsValidOutputDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(OutputDirectory))
                {
                    return false;
                }

                if (!Directory.Exists(OutputDirectory))
                {
                    return false;
                }

                return true;
            }
        }

        public string WorkingPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return UtilPath.GetRegularPath(new DirectoryInfo(UtilText.Format("{0}/Working/", OutputDirectory)).FullName);
            }
        }

        public string OutputPackagePath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return UtilPath.GetRegularPath(new DirectoryInfo(UtilText.Format("{0}/Package/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString())).FullName);
            }
        }

        public string OutputFullPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return UtilPath.GetRegularPath(new DirectoryInfo(UtilText.Format("{0}/Full/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString())).FullName);
            }
        }

        public string BuildReportPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return UtilPath.GetRegularPath(new DirectoryInfo(UtilText.Format("{0}/BuildReport/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString())).FullName);
            }
        }

        public event Action<int, int> OnLoadingResource = null;

        public event Action<int, int> OnLoadingAsset = null;

        public event Action OnLoadCompleted = null;

        public event Action<int, int> OnAnalyzingAsset = null;

        public event Action OnAnalyzeCompleted = null;

        public event Func<string, float, bool> ProcessingAssetBundle = null;

        public event Func<string, float, bool> ProcessingBinary = null;

        public event Action<Platform> ProcessResourceComplete = null;

        public event Action<string> BuildResourceError = null;

        public bool Load()
        {
            if (!File.Exists(m_ConfigurationPath))
            {
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("EPloy");
                XmlNode xmlEditor = xmlRoot.SelectSingleNode("ResourceBuilder");
                XmlNode xmlSettings = xmlEditor.SelectSingleNode("Settings");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;

                xmlNodeList = xmlSettings.ChildNodes;
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    xmlNode = xmlNodeList.Item(i);
                    switch (xmlNode.Name)
                    {
                        case "InternalResourceVersion":
                            InternalResourceVersion = int.Parse(xmlNode.InnerText) + 1;
                            break;

                        case "Platforms":
                            Platforms = (Platform)int.Parse(xmlNode.InnerText);
                            break;

                        case "AssetBundleZip":
                            AssetBundleZip = (AssetBundleZipType)byte.Parse(xmlNode.InnerText);
                            break;

                        case "ZipHelperTypeName":
                            ZipHelperTypeName = xmlNode.InnerText;
                            break;

                        case "AdditionalZipSelected":
                            AdditionalZipSelected = bool.Parse(xmlNode.InnerText);
                            break;

                        case "ForceRebuildAssetBundleSelected":
                            ForceRebuildAssetBundleSelected = bool.Parse(xmlNode.InnerText);
                            break;

                        case "BuildEventHandlerTypeName":
                            BuildEventHandlerTypeName = xmlNode.InnerText;
                            break;

                        case "OutputDirectory":
                            OutputDirectory = xmlNode.InnerText;
                            break;

                        case "OutputPackageSelected":
                            OutputPackageSelected = bool.Parse(xmlNode.InnerText);
                            break;

                        case "OutputFullSelected":
                            OutputFullSelected = bool.Parse(xmlNode.InnerText);
                            break;
                    }
                }
            }
            catch
            {
                File.Delete(m_ConfigurationPath);
                return false;
            }

            return true;
        }

        public bool Save()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                XmlElement xmlRoot = xmlDocument.CreateElement("EPloy");
                xmlDocument.AppendChild(xmlRoot);

                XmlElement xmlBuilder = xmlDocument.CreateElement("ResourceBuilder");
                xmlRoot.AppendChild(xmlBuilder);

                XmlElement xmlSettings = xmlDocument.CreateElement("Settings");
                xmlBuilder.AppendChild(xmlSettings);

                XmlElement xmlElement = null;

                xmlElement = xmlDocument.CreateElement("InternalResourceVersion");
                xmlElement.InnerText = InternalResourceVersion.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("Platforms");
                xmlElement.InnerText = ((int)Platforms).ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("AssetBundleZip");
                xmlElement.InnerText = ((byte)AssetBundleZip).ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("ZipHelperTypeName");
                xmlElement.InnerText = ZipHelperTypeName;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("AdditionalZipSelected");
                xmlElement.InnerText = AdditionalZipSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("ForceRebuildAssetBundleSelected");
                xmlElement.InnerText = ForceRebuildAssetBundleSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("BuildEventHandlerTypeName");
                xmlElement.InnerText = BuildEventHandlerTypeName;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("OutputDirectory");
                xmlElement.InnerText = OutputDirectory;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("OutputPackageSelected");
                xmlElement.InnerText = OutputPackageSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("OutputFullSelected");
                xmlElement.InnerText = OutputFullSelected.ToString();
                xmlSettings.AppendChild(xmlElement);

                string configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath);
                if (!Directory.Exists(configurationDirectoryName))
                {
                    Directory.CreateDirectory(configurationDirectoryName);
                }

                xmlDocument.Save(m_ConfigurationPath);
                AssetDatabase.Refresh();
                return true;
            }
            catch
            {
                if (File.Exists(m_ConfigurationPath))
                {
                    File.Delete(m_ConfigurationPath);
                }

                return false;
            }
        }

        public bool IsPlatformSelected(Platform platform)
        {
            return (Platforms & platform) != 0;
        }

        public void SelectPlatform(Platform platform, bool selected)
        {
            if (selected)
            {
                Platforms |= platform;
            }
            else
            {
                Platforms &= ~platform;
            }
        }

        public bool BuildResources()
        {
            if (!IsValidOutputDirectory)
            {
                return false;
            }

            if (Directory.Exists(OutputPackagePath))
            {
                Directory.Delete(OutputPackagePath, true);
            }

            Directory.CreateDirectory(OutputPackagePath);

            if (Directory.Exists(OutputFullPath))
            {
                Directory.Delete(OutputFullPath, true);
            }

            Directory.CreateDirectory(OutputFullPath);

            if (Directory.Exists(BuildReportPath))
            {
                Directory.Delete(BuildReportPath, true);
            }

            Directory.CreateDirectory(BuildReportPath);

            BuildAssetBundleOptions buildAssetBundleOptions = GetBuildAssetBundleOptions();
            m_BuildReport.Initialize(BuildReportPath, ProductName, CompanyName, GameIdentifier, Version, UnityVersion, ApplicableGameVersion, InternalResourceVersion,
                Platforms, AssetBundleZip, ZipHelperTypeName, AdditionalZipSelected, ForceRebuildAssetBundleSelected, BuildEventHandlerTypeName, OutputDirectory, buildAssetBundleOptions, m_ResourceDatas);

            try
            {
                m_BuildReport.LogInfo("Build Start Time: {0}", DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
                m_BuildReport.LogInfo("Start prepare resource collection...");
                if (!m_ResourceCollection.Load())
                {
                    m_BuildReport.LogError("Can not parse 'ResCollection.xml', please use 'Resource Editor' tool first.");
                    m_BuildReport.SaveReport();
                    return false;
                }

                if (Platforms == Platform.Undefined)
                {
                    m_BuildReport.LogError("Platform undefined.");
                    m_BuildReport.SaveReport();
                    return false;
                }

                m_BuildReport.LogInfo("Prepare resource collection complete.");
                m_BuildReport.LogInfo("Start analyze assets dependency...");

                m_ResourceAnalyzerController.Analyze();

                m_BuildReport.LogInfo("Analyze assets dependency complete.");
                m_BuildReport.LogInfo("Start prepare build data...");

                AssetBundleBuild[] assetBundleBuildDatas = null;
                ResourceData[] assetBundleResourceDatas = null;
                ResourceData[] binaryResourceDatas = null;
                if (!PrepareBuildData(out assetBundleBuildDatas, out assetBundleResourceDatas, out binaryResourceDatas))
                {
                    m_BuildReport.LogError("Prepare resource build data failure.");
                    m_BuildReport.SaveReport();
                    return false;
                }
                BuildResources(Platforms, assetBundleBuildDatas, buildAssetBundleOptions, assetBundleResourceDatas, binaryResourceDatas);
                m_BuildReport.LogInfo("Prepare resource build data complete.");
                m_BuildReport.LogInfo("Start build resources for selected platforms...");
                m_BuildReport.LogInfo("Build resources for selected platforms complete.");
                m_BuildReport.SaveReport();

                return true;
            }
            catch (Exception exception)
            {
                string errorMessage = exception.ToString();
                m_BuildReport.LogFatal(errorMessage);
                m_BuildReport.SaveReport();
                if (BuildResourceError != null)
                {
                    BuildResourceError(errorMessage);
                }

                return false;
            }
            finally
            {
                // m_OutputPackageFileSystems.Clear();
                // m_OutputPackedFileSystems.Clear();
                // if (m_FileSystemManager != null)
                // {
                //     GameFrameworkEntry.Shutdown();
                //     m_FileSystemManager = null;
                // }
            }
        }

        private bool BuildResources(Platform platform, AssetBundleBuild[] assetBundleBuildDatas, BuildAssetBundleOptions buildAssetBundleOptions, ResourceData[] assetBundleResourceDatas, ResourceData[] binaryResourceDatas)
        {
            if (!IsPlatformSelected(platform))
            {
                return true;
            }

            string platformName = platform.ToString();
            m_BuildReport.LogInfo("Start build resources for '{0}'...", platformName);

            string workingPath = UtilText.Format("{0}{1}/", WorkingPath, platformName);
            m_BuildReport.LogInfo("Working path is '{0}'.", workingPath);

            string outputPackagePath = UtilText.Format("{0}{1}/", OutputPackagePath, platformName);
            if (OutputPackageSelected)
            {
                Directory.CreateDirectory(outputPackagePath);
                m_BuildReport.LogInfo("Output package is selected, path is '{0}'.", outputPackagePath);
            }
            else
            {
                m_BuildReport.LogInfo("Output package is not selected.");
            }

            string outputFullPath = UtilText.Format("{0}{1}/", OutputFullPath, platformName);
            if (OutputFullSelected)
            {
                Directory.CreateDirectory(outputFullPath);
                m_BuildReport.LogInfo("Output full is selected, path is '{0}'.", outputFullPath);
            }
            else
            {
                m_BuildReport.LogInfo("Output full is not selected.");
            }

            m_BuildReport.LogInfo("Output packed is not selected.");

            // Clean working path
            List<string> validNames = new List<string>();
            foreach (ResourceData assetBundleResourceData in assetBundleResourceDatas)
            {
                validNames.Add(assetBundleResourceData.Name.ToLowerInvariant());
            }

            if (Directory.Exists(workingPath))
            {
                Uri workingUri = new Uri(workingPath, UriKind.Absolute);
                string[] fileNames = Directory.GetFiles(workingPath, "*", SearchOption.AllDirectories);
                foreach (string fileName in fileNames)
                {
                    if (fileName.EndsWith(".manifest", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    string relativeName = workingUri.MakeRelativeUri(new Uri(fileName, UriKind.Absolute)).ToString();
                    if (!validNames.Contains(relativeName))
                    {
                        File.Delete(fileName);
                    }
                }

                string[] manifestNames = Directory.GetFiles(workingPath, "*.manifest", SearchOption.AllDirectories);
                foreach (string manifestName in manifestNames)
                {
                    if (!File.Exists(manifestName.Substring(0, manifestName.LastIndexOf('.'))))
                    {
                        File.Delete(manifestName);
                    }
                }

                UtilPath.RemoveEmptyDirectory(workingPath);
            }

            if (!Directory.Exists(workingPath))
            {
                Directory.CreateDirectory(workingPath);
            }

            // Build AssetBundles
            m_BuildReport.LogInfo("Unity start build asset bundles for '{0}'...", platformName);
            AssetBundleManifest assetBundleManifest= BuildPipeline.BuildAssetBundles(workingPath, assetBundleBuildDatas, buildAssetBundleOptions, GetBuildTarget(platform));
            if (assetBundleManifest==null)
            {
                m_BuildReport.LogInfo("Unity build asset bundles for '{0}' failure", platformName);
                return false;
            }
            m_BuildReport.LogInfo("Unity build asset bundles for '{0}' complete.", platformName);

            // Process AssetBundles
            for (int i = 0; i < assetBundleResourceDatas.Length; i++)
            {
                string name = assetBundleResourceDatas[i].Name;
                if (ProcessingAssetBundle != null)
                {
                    if (ProcessingAssetBundle(name, (float)(i + 1) / assetBundleResourceDatas.Length))
                    {
                        m_BuildReport.LogWarning("The build has been canceled by user.");

                        return false;
                    }
                }

                m_BuildReport.LogInfo("Start process asset bundle '{0}' for '{1}'...", name, platformName);

                if (!ProcessAssetBundle(platform, workingPath, outputPackagePath, outputFullPath, AdditionalZipSelected, assetBundleResourceDatas[i].Name))
                {
                    return false;
                }

                m_BuildReport.LogInfo("Process asset bundle '{0}' for '{1}' complete.", name, platformName);
            }

            // Process Binaries
            for (int i = 0; i < binaryResourceDatas.Length; i++)
            {
                string name = assetBundleResourceDatas[i].Name;
                if (ProcessingBinary != null)
                {
                    if (ProcessingBinary(name, (float)(i + 1) / binaryResourceDatas.Length))
                    {
                        m_BuildReport.LogWarning("The build has been canceled by user.");
                        return false;
                    }
                }

                m_BuildReport.LogInfo("Start process binary '{0}' for '{1}'...", name, platformName);

                if (!ProcessBinary(platform, workingPath, outputPackagePath, outputFullPath, AdditionalZipSelected, binaryResourceDatas[i].Name))
                {
                    return false;
                }

                m_BuildReport.LogInfo("Process binary '{0}' for '{1}' complete.", name, platformName);
            }

            if (OutputFullSelected)
            {
                VersionListData versionListData = ProcessUpdatableVersionList(outputFullPath, platform);
                m_BuildReport.LogInfo("Process updatable version list for '{0}' complete, updatable version list path is '{1}', length is '{2}', hash code is '{3}[0x{3:X8}]', compressed length is '{4}', compressed hash code is '{5}[0x{5:X8}]'.", platformName, versionListData.Path, versionListData.Length.ToString(), versionListData.HashCode, versionListData.CompressedLength.ToString(), versionListData.CompressedHashCode);
            }
            if (OutputPackageSelected)
            {
                VersionListData versionListData = ProcessPackageVersionList(outputPackagePath, platform);
                m_BuildReport.LogInfo("Process OutputPackageSelected version list for '{0}' complete, updatable version list path is '{1}', length is '{2}', hash code is '{3}[0x{3:X8}]', compressed length is '{4}', compressed hash code is '{5}[0x{5:X8}]'.", platformName, versionListData.Path, versionListData.Length.ToString(), versionListData.HashCode, versionListData.CompressedLength.ToString(), versionListData.CompressedHashCode);
            }

            if (ProcessResourceComplete != null)
            {
                ProcessResourceComplete(platform);
            }

            m_BuildReport.LogInfo("Build resources for '{0}' success.", platformName);
            return true;
        }

        private bool ProcessAssetBundle(Platform platform, string workingPath, string outputPackagePath, string outputFullPath, bool additionalZipSelected, string fullName)
        {
            ResourceData resourceData = m_ResourceDatas[fullName];
            string workingName = UtilPath.GetRegularPath(Path.Combine(workingPath, fullName.ToLowerInvariant()));

            byte[] bytes = File.ReadAllBytes(workingName);
            int length = bytes.Length;
            int hashCode = UtilVerifier.GetCrc32(bytes);
            int compressedLength = length;
            int compressedHashCode = hashCode;

            byte[] hashBytes = UtilConverter.GetBytes(hashCode);
            if (resourceData.LoadType == LoadType.LoadFromMemory)
            {
                bytes = UtilEncryption.GetQuickXorBytes(bytes, hashBytes);
            }
            else if (resourceData.LoadType == LoadType.LoadFromBinary)
            {
                bytes = UtilEncryption.GetXorBytes(bytes, hashBytes);
            }

            return ProcessOutput(platform, outputPackagePath, outputFullPath, additionalZipSelected, fullName, resourceData, bytes, length, hashCode, compressedLength, compressedHashCode);
        }

        private bool ProcessBinary(Platform platform, string workingPath, string outputPackagePath, string outputFullPath, bool additionalZipSelected, string fullName)
        {
            ResourceData resourceData = m_ResourceDatas[fullName];
            string assetName = resourceData.GetAssetNames()[0];
            string assetPath = UtilPath.GetRegularPath(Application.dataPath.Substring(0, Application.dataPath.Length - AssetsStringLength) + assetName);

            byte[] bytes = File.ReadAllBytes(assetPath);
            int length = bytes.Length;
            int hashCode = UtilVerifier.GetCrc32(bytes);
            int compressedLength = length;
            int compressedHashCode = hashCode;

            byte[] hashBytes = UtilConverter.GetBytes(hashCode);
            if (resourceData.LoadType == LoadType.LoadFromMemory)
            {
                bytes = UtilEncryption.GetQuickXorBytes(bytes, hashBytes);
            }
            else if (resourceData.LoadType == LoadType.LoadFromBinary)
            {
                bytes = UtilEncryption.GetXorBytes(bytes, hashBytes);
            }

            return ProcessOutput(platform, outputPackagePath, outputFullPath, additionalZipSelected, fullName ,resourceData, bytes, length, hashCode, compressedLength, compressedHashCode);
        }

        private VersionListData ProcessUpdatableVersionList(string outputFullPath, Platform platform)
        {
            Asset[] originalAssets = m_ResourceCollection.GetAssets();
            UpdatableVersionList.Asset[] assets = new UpdatableVersionList.Asset[originalAssets.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                Asset originalAsset = originalAssets[i];
                assets[i] = new UpdatableVersionList.Asset(originalAsset.Name, GetDependencyAssetIndexes(originalAsset.Name));
            }

            SortedDictionary<string, ResourceData>.ValueCollection resourceDatas = m_ResourceDatas.Values;

            int index = 0;
            UpdatableVersionList.Resource[] resources = new UpdatableVersionList.Resource[m_ResourceCollection.ResourceCount];
            foreach (ResourceData resourceData in resourceDatas)
            {
                ResourceCode resourceCode = resourceData.GetCode(platform);
                resources[index++] = new UpdatableVersionList.Resource(resourceData.Name, (byte)resourceData.LoadType, resourceCode.Length, resourceCode.HashCode, resourceCode.CompressedLength, resourceCode.CompressedHashCode, GetAssetIndexes(resourceData));
            }

            string[] resourceGroupNames = GetResourceGroupNames(resourceDatas);
            UpdatableVersionList.ResourceGroup[] resourceGroups = new UpdatableVersionList.ResourceGroup[resourceGroupNames.Length];
            for (int i = 0; i < resourceGroups.Length; i++)
            {
                resourceGroups[i] = new UpdatableVersionList.ResourceGroup(resourceGroupNames[i], GetResourceIndexesFromResourceGroup(resourceDatas, resourceGroupNames[i]));
            }

            UpdatableVersionList versionList = new UpdatableVersionList(ApplicableGameVersion, InternalResourceVersion, assets, resources, resourceGroups);
            UpdatableVersionListSerializer serializer = new UpdatableVersionListSerializer();
            string updatableVersionListPath = UtilPath.GetRegularPath(Path.Combine(outputFullPath, ConfigVersion.RemoteVersionListFileName));
            using (FileStream fileStream = new FileStream(updatableVersionListPath, FileMode.Create, FileAccess.Write))
            {
                if (!serializer.Serialize(fileStream, versionList))
                {
                    Log.Fatal("Serialize updatable version list failure.");
                }
            }

            byte[] bytes = File.ReadAllBytes(updatableVersionListPath);
            int length = bytes.Length;
            int hashCode = UtilVerifier.GetCrc32(bytes);
            bytes = EditorZip.Compress(bytes);
            int compressedLength = bytes.Length;
            File.WriteAllBytes(updatableVersionListPath, bytes);
            int compressedHashCode = UtilVerifier.GetCrc32(bytes);
            int dotPosition = ConfigVersion.RemoteVersionListFileName.LastIndexOf('.');
            string versionListFullNameWithCrc32 = UtilText.Format("{0}.{2:x8}.{1}", ConfigVersion.RemoteVersionListFileName.Substring(0, dotPosition), ConfigVersion.RemoteVersionListFileName.Substring(dotPosition + 1), hashCode);
            string updatableVersionListPathWithCrc32 = UtilPath.GetRegularPath(Path.Combine(outputFullPath, versionListFullNameWithCrc32));
            File.Move(updatableVersionListPath, updatableVersionListPathWithCrc32);

            return new VersionListData(updatableVersionListPathWithCrc32, length, hashCode, compressedLength, compressedHashCode);
        }

        private VersionListData ProcessPackageVersionList(string outputPackagePath, Platform platform)
        {
            Asset[] originalAssets = m_ResourceCollection.GetAssets();
            UpdatableVersionList.Asset[] assets = new UpdatableVersionList.Asset[originalAssets.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                Asset originalAsset = originalAssets[i];
                assets[i] = new UpdatableVersionList.Asset(originalAsset.Name, GetDependencyAssetIndexes(originalAsset.Name));
            }

            SortedDictionary<string, ResourceData>.ValueCollection resourceDatas = m_ResourceDatas.Values;

            int index = 0;
            UpdatableVersionList.Resource[] resources = new UpdatableVersionList.Resource[m_ResourceCollection.ResourceCount];
            foreach (ResourceData resourceData in resourceDatas)
            {
                ResourceCode resourceCode = resourceData.GetCode(platform);
                resources[index++] = new UpdatableVersionList.Resource(resourceData.Name, (byte)resourceData.LoadType, resourceCode.Length, resourceCode.HashCode, resourceCode.CompressedLength, resourceCode.CompressedHashCode, GetAssetIndexes(resourceData));
            }

            string[] resourceGroupNames = GetResourceGroupNames(resourceDatas);
            UpdatableVersionList.ResourceGroup[] resourceGroups = new UpdatableVersionList.ResourceGroup[resourceGroupNames.Length];
            for (int i = 0; i < resourceGroups.Length; i++)
            {
                resourceGroups[i] = new UpdatableVersionList.ResourceGroup(resourceGroupNames[i], GetResourceIndexesFromResourceGroup(resourceDatas, resourceGroupNames[i]));
            }

            UpdatableVersionList versionList = new UpdatableVersionList(ApplicableGameVersion, InternalResourceVersion, assets, resources, resourceGroups);
            UpdatableVersionListSerializer serializer = new UpdatableVersionListSerializer();
            string updatableVersionListPath = UtilPath.GetRegularPath(Path.Combine(outputPackagePath, ConfigVersion.RemoteVersionListFileName));
            using (FileStream fileStream = new FileStream(updatableVersionListPath, FileMode.Create, FileAccess.Write))
            {
                if (!serializer.Serialize(fileStream, versionList))
                {
                    Log.Fatal("Serialize updatable version list failure.");
                }
            }

            byte[] bytes = File.ReadAllBytes(updatableVersionListPath);
            int length = bytes.Length;
            int hashCode = UtilVerifier.GetCrc32(bytes);
            bytes = EditorZip.Compress(bytes);
            int compressedLength = bytes.Length;
            File.WriteAllBytes(updatableVersionListPath, bytes);
            int compressedHashCode = UtilVerifier.GetCrc32(bytes);
            int dotPosition = ConfigVersion.RemoteVersionListFileName.LastIndexOf('.');
            string versionListFullNameWithCrc32 = UtilText.Format("{0}.{1}", ConfigVersion.RemoteVersionListFileName.Substring(0, dotPosition), ConfigVersion.RemoteVersionListFileName.Substring(dotPosition + 1));
            string updatableVersionListPathWithCrc32 = UtilPath.GetRegularPath(Path.Combine(outputPackagePath, versionListFullNameWithCrc32));
            File.Move(updatableVersionListPath, updatableVersionListPathWithCrc32);

            return new VersionListData(updatableVersionListPathWithCrc32, length, hashCode, compressedLength, compressedHashCode);
        }

        private int[] GetDependencyAssetIndexes(string assetName)
        {
            List<int> dependencyAssetIndexes = new List<int>();
            Asset[] assets = m_ResourceCollection.GetAssets();
            DependencyData dependencyData = m_ResourceAnalyzerController.GetDependencyData(assetName);
            foreach (Asset dependencyAsset in dependencyData.GetDependencyAssets())
            {
                for (int i = 0; i < assets.Length; i++)
                {
                    if (assets[i] == dependencyAsset)
                    {
                        dependencyAssetIndexes.Add(i);
                        break;
                    }
                }
            }

            dependencyAssetIndexes.Sort();
            return dependencyAssetIndexes.ToArray();
        }

        private int[] GetAssetIndexes(ResourceData resourceData)
        {
            Asset[] assets = m_ResourceCollection.GetAssets();
            string[] assetGuids = resourceData.GetAssetGuids();
            int[] assetIndexes = new int[assetGuids.Length];
            for (int i = 0; i < assetGuids.Length; i++)
            {
                Asset asset =m_ResourceCollection.GetAsset(assetGuids[i]);
                for (int j = 0; j < assets.Length; j++)
                {
                    if (assets[j]== asset)
                    {
                        assetIndexes[i] = j;
                        break;
                    }
                
                }
                if (assetIndexes[i] < 0)
                {
                    Log.Fatal("Asset is invalid.");
                }
            }

            return assetIndexes;
        }

        private string[] GetResourceGroupNames(IEnumerable<ResourceData> resourceDatas)
        {
            HashSet<string> resourceGroupNames = new HashSet<string>();
            foreach (ResourceData resourceData in resourceDatas)
            {
                foreach (string resourceGroup in resourceData.GetResourceGroups())
                {
                    resourceGroupNames.Add(resourceGroup);
                }
            }

            return resourceGroupNames.OrderBy(x => x).ToArray();
        }

        private int[] GetResourceIndexesFromResourceGroup(IEnumerable<ResourceData> resourceDatas, string resourceGroupName)
        {
            int index = 0;
            List<int> resourceIndexes = new List<int>();
            foreach (ResourceData resourceData in resourceDatas)
            {
                foreach (string resourceGroup in resourceData.GetResourceGroups())
                {
                    if (resourceGroup == resourceGroupName)
                    {
                        resourceIndexes.Add(index);
                        break;
                    }
                }

                index++;
            }

            resourceIndexes.Sort();
            return resourceIndexes.ToArray();
        }

        private bool ProcessOutput(Platform platform, string outputPackagePath, string outputFullPath, bool additionalZipSelected, string name, ResourceData resourceData, byte[] bytes, int length, int hashCode, int compressedLength, int compressedHashCode)
        {
            string fullNameWithExtension = name;

            if (OutputPackageSelected)
            {
                string packagePath = UtilPath.GetRegularPath(Path.Combine(outputPackagePath, fullNameWithExtension));
                string packageDirectoryName = Path.GetDirectoryName(packagePath);
                if (!Directory.Exists(packageDirectoryName))
                {
                    Directory.CreateDirectory(packageDirectoryName);
                }
                if (additionalZipSelected)
                {
                    byte[] compressedBytes = EditorZip.Compress(bytes);
                    compressedLength = compressedBytes.Length;
                    compressedHashCode = UtilVerifier.GetCrc32(compressedBytes);
                    File.WriteAllBytes(packagePath, compressedBytes);
                }
                else
                {
                    File.WriteAllBytes(packagePath, bytes);
                }
            }

            if (OutputFullSelected)
            {
                string fullNameWithCrc32AndExtension = UtilText.Format("{0}.{1:x8}.{2}", name, hashCode, ConfigVersion.DefaultExtension);
                string fullPath = UtilPath.GetRegularPath(Path.Combine(outputFullPath, fullNameWithCrc32AndExtension));
                string fullDirectoryName = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(fullDirectoryName))
                {
                    Directory.CreateDirectory(fullDirectoryName);
                }

                if (additionalZipSelected)
                {
                    byte[] compressedBytes = EditorZip.Compress(bytes);
                    compressedLength = compressedBytes.Length;
                    compressedHashCode = UtilVerifier.GetCrc32(compressedBytes);
                    File.WriteAllBytes(fullPath, compressedBytes);
                }
                else
                {
                    File.WriteAllBytes(fullPath, bytes);
                }
            }

            resourceData.AddCode(platform, length, hashCode, compressedLength, compressedHashCode);
            return true;
        }

        private BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.DeterministicAssetBundle;

            if (ForceRebuildAssetBundleSelected)
            {
                buildOptions |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }

            if (AssetBundleZip == AssetBundleZipType.UnZip)
            {
                buildOptions |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }
            else if (AssetBundleZip == AssetBundleZipType.LZ4)
            {
                // buildOptions |= BuildAssetBundleOptions.ChunkBasedZip;
            }

            return buildOptions;
        }

        private bool PrepareBuildData(out AssetBundleBuild[] assetBundleBuildDatas, out ResourceData[] assetBundleResourceDatas, out ResourceData[] binaryResourceDatas)
        {
            assetBundleBuildDatas = null;
            assetBundleResourceDatas = null;
            binaryResourceDatas = null;
            m_ResourceDatas.Clear();

            Resource[] resources = m_ResourceCollection.GetResources();
            foreach (Resource resource in resources)
            {
                m_ResourceDatas.Add(resource.Name, new ResourceData(resource.Name, resource.Extension, resource.LoadType, resource.GetResourceGroups()));
            }

            Asset[] assets = m_ResourceCollection.GetAssets();
            foreach (Asset asset in assets)
            {
                string assetName = asset.Name;
                if (string.IsNullOrEmpty(assetName))
                {
                    m_BuildReport.LogError("Can not find asset by guid '{0}'.", asset.Guid);
                    return false;
                }

                string assetFileFullName = Application.dataPath.Substring(0, Application.dataPath.Length - AssetsStringLength) + assetName;
                if (!File.Exists(assetFileFullName))
                {
                    m_BuildReport.LogError("Can not find asset '{0}'.", assetFileFullName);
                    return false;
                }

                byte[] assetBytes = File.ReadAllBytes(assetFileFullName);
                int assetHashCode = UtilVerifier.GetCrc32(assetBytes);

                List<string> dependencyAssetNames = new List<string>();
                DependencyData dependencyData = m_ResourceAnalyzerController.GetDependencyData(assetName);
                Asset[] dependencyAssets = dependencyData.GetDependencyAssets();
                foreach (Asset dependencyAsset in dependencyAssets)
                {
                    dependencyAssetNames.Add(dependencyAsset.Name);
                }

                dependencyAssetNames.Sort();

                m_ResourceDatas[asset.Resource.Name].AddAssetData(asset.Guid, assetName, assetBytes.Length, assetHashCode, dependencyAssetNames.ToArray());
            }

            List<AssetBundleBuild> assetBundleBuildDataList = new List<AssetBundleBuild>();
            List<ResourceData> assetBundleResourceDataList = new List<ResourceData>();
            List<ResourceData> binaryResourceDataList = new List<ResourceData>();
            foreach (ResourceData resourceData in m_ResourceDatas.Values)
            {
                if (resourceData.AssetCount <= 0)
                {
                    m_BuildReport.LogError("Resource '{0}' has no asset.", resourceData.Name);
                    return false;
                }

                if (resourceData.IsLoadFromBinary)
                {
                    binaryResourceDataList.Add(resourceData);
                }
                else
                {
                    assetBundleResourceDataList.Add(resourceData);

                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = resourceData.Name;
                    build.assetNames = resourceData.GetAssetNames();
                    assetBundleBuildDataList.Add(build);
                }
            }

            assetBundleBuildDatas = assetBundleBuildDataList.ToArray();
            assetBundleResourceDatas = assetBundleResourceDataList.ToArray();
            binaryResourceDatas = binaryResourceDataList.ToArray();
            return true;
        }

        private static BuildTarget GetBuildTarget(Platform platform)
        {
            switch (platform)
            {
                case Platform.Windows:
                    return BuildTarget.StandaloneWindows;

                case Platform.Windows64:
                    return BuildTarget.StandaloneWindows64;

                case Platform.MacOS:
#if UNITY_2017_3_OR_NEWER
                    return BuildTarget.StandaloneOSX;
#else
                    return BuildTarget.StandaloneOSXUniversal;
#endif
                case Platform.Linux:
                    return BuildTarget.StandaloneLinux64;

                case Platform.IOS:
                    return BuildTarget.iOS;

                case Platform.Android:
                    return BuildTarget.Android;

                case Platform.WindowsStore:
                    return BuildTarget.WSAPlayer;

                case Platform.WebGL:
                    return BuildTarget.WebGL;

                default:
                    Log.Fatal("Platform is invalid.");
                    return default(BuildTarget);
            }
        }
    }
}
