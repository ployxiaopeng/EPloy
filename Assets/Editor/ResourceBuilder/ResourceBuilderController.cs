using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;
using EPloy.Res;

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
            m_ConfigurationPath = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, EPloyEditorPath.ResBuilder));

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

        public string GameFrameworkVersion
        {
            get
            {
                return "GameFramework";
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

                return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/Working/", OutputDirectory)).FullName);
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

                return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/Package/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString())).FullName);
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

                return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/Full/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString())).FullName);
            }
        }

        public string OutputPackedPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/Packed/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString())).FullName);
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

                return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/BuildReport/{1}_{2}/", OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString())).FullName);
            }
        }

        public event EPloyAction<int, int> OnLoadingResource = null;

        public event EPloyAction<int, int> OnLoadingAsset = null;

        public event EPloyAction OnLoadCompleted = null;

        public event EPloyAction<int, int> OnAnalyzingAsset = null;

        public event EPloyAction OnAnalyzeCompleted = null;

        public event EPloyFunc<string, float, bool> ProcessingAssetBundle = null;

        public event EPloyFunc<string, float, bool> ProcessingBinary = null;

        public event EPloyAction<Platform> ProcessResourceComplete = null;

        public event EPloyAction<string> BuildResourceError = null;

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

            if (Directory.Exists(OutputPackedPath))
            {
                Directory.Delete(OutputPackedPath, true);
            }

            Directory.CreateDirectory(OutputPackedPath);

            if (Directory.Exists(BuildReportPath))
            {
                Directory.Delete(BuildReportPath, true);
            }

            Directory.CreateDirectory(BuildReportPath);

            BuildAssetBundleOptions buildAssetBundleOptions = GetBuildAssetBundleOptions();
            m_BuildReport.Initialize(BuildReportPath, ProductName, CompanyName, GameIdentifier, GameFrameworkVersion, UnityVersion, ApplicableGameVersion, InternalResourceVersion,
                Platforms, AssetBundleZip, ZipHelperTypeName, AdditionalZipSelected, ForceRebuildAssetBundleSelected, BuildEventHandlerTypeName, OutputDirectory, buildAssetBundleOptions, m_ResourceDatas);

            try
            {
                m_BuildReport.LogInfo("Build Start Time: {0}", DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
                m_BuildReport.LogInfo("Start prepare resource collection...");
                if (!m_ResourceCollection.Load())
                {
                    m_BuildReport.LogError("Can not parse 'ResourceCollection.xml', please use 'Resource Editor' tool first.");
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

            string workingPath = Utility.Text.Format("{0}{1}/", WorkingPath, platformName);
            m_BuildReport.LogInfo("Working path is '{0}'.", workingPath);

            string outputPackagePath = Utility.Text.Format("{0}{1}/", OutputPackagePath, platformName);
            if (OutputPackageSelected)
            {
                Directory.CreateDirectory(outputPackagePath);
                m_BuildReport.LogInfo("Output package is selected, path is '{0}'.", outputPackagePath);
            }
            else
            {
                m_BuildReport.LogInfo("Output package is not selected.");
            }

            string outputFullPath = Utility.Text.Format("{0}{1}/", OutputFullPath, platformName);
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
                validNames.Add(GetResourceFullName(assetBundleResourceData.Name, assetBundleResourceData.Variant).ToLowerInvariant());
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

                Utility.Path.RemoveEmptyDirectory(workingPath);
            }

            if (!Directory.Exists(workingPath))
            {
                Directory.CreateDirectory(workingPath);
            }

            // Build AssetBundles
            m_BuildReport.LogInfo("Unity start build asset bundles for '{0}'...", platformName);
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(workingPath, assetBundleBuildDatas, buildAssetBundleOptions, GetBuildTarget(platform));
            if (assetBundleManifest == null)
            {
                m_BuildReport.LogError("Build asset bundles for '{0}' failure.", platformName);
                return false;
            }

            m_BuildReport.LogInfo("Unity build asset bundles for '{0}' complete.", platformName);

            // Process AssetBundles
            for (int i = 0; i < assetBundleResourceDatas.Length; i++)
            {
                string fullName = GetResourceFullName(assetBundleResourceDatas[i].Name, assetBundleResourceDatas[i].Variant);
                if (ProcessingAssetBundle != null)
                {
                    if (ProcessingAssetBundle(fullName, (float)(i + 1) / assetBundleResourceDatas.Length))
                    {
                        m_BuildReport.LogWarning("The build has been canceled by user.");

                        return false;
                    }
                }

                m_BuildReport.LogInfo("Start process asset bundle '{0}' for '{1}'...", fullName, platformName);

                if (!ProcessAssetBundle(platform, workingPath, outputPackagePath, outputFullPath, AdditionalZipSelected, assetBundleResourceDatas[i].Name, assetBundleResourceDatas[i].Variant))
                {
                    return false;
                }

                m_BuildReport.LogInfo("Process asset bundle '{0}' for '{1}' complete.", fullName, platformName);
            }

            // Process Binaries
            for (int i = 0; i < binaryResourceDatas.Length; i++)
            {
                string fullName = GetResourceFullName(binaryResourceDatas[i].Name, binaryResourceDatas[i].Variant);
                if (ProcessingBinary != null)
                {
                    if (ProcessingBinary(fullName, (float)(i + 1) / binaryResourceDatas.Length))
                    {
                        m_BuildReport.LogWarning("The build has been canceled by user.");
                        return false;
                    }
                }

                m_BuildReport.LogInfo("Start process binary '{0}' for '{1}'...", fullName, platformName);

                if (!ProcessBinary(platform, workingPath, outputPackagePath, outputFullPath, AdditionalZipSelected, binaryResourceDatas[i].Name, binaryResourceDatas[i].Variant))
                {
                    return false;
                }

                m_BuildReport.LogInfo("Process binary '{0}' for '{1}' complete.", fullName, platformName);
            }

            if (OutputFullSelected)
            {
                VersionListData versionListData = ProcessUpdatableVersionList(outputFullPath, platform);
                m_BuildReport.LogInfo("Process updatable version list for '{0}' complete, updatable version list path is '{1}', length is '{2}', hash code is '{3}[0x{3:X8}]', compressed length is '{4}', compressed hash code is '{5}[0x{5:X8}]'.", platformName, versionListData.Path, versionListData.Length.ToString(), versionListData.HashCode, versionListData.CompressedLength.ToString(), versionListData.CompressedHashCode);
            }

            if (ProcessResourceComplete != null)
            {
                ProcessResourceComplete(platform);
            }

            m_BuildReport.LogInfo("Build resources for '{0}' success.", platformName);
            return true;
        }

        private bool ProcessAssetBundle(Platform platform, string workingPath, string outputPackagePath, string outputFullPath, bool additionalZipSelected, string name, string variant)
        {
            string fullName = GetResourceFullName(name, variant);
            ResourceData resourceData = m_ResourceDatas[fullName];
            string workingName = Utility.Path.GetRegularPath(Path.Combine(workingPath, fullName.ToLowerInvariant()));

            byte[] bytes = File.ReadAllBytes(workingName);
            int length = bytes.Length;
            int hashCode = Utility.Verifier.GetCrc32(bytes);
            int compressedLength = length;
            int compressedHashCode = hashCode;

            byte[] hashBytes = Utility.Converter.GetBytes(hashCode);
            if (resourceData.LoadType == LoadType.LoadFromMemory)
            {
                bytes = Utility.Encryption.GetQuickXorBytes(bytes, hashBytes);
            }
            else if (resourceData.LoadType == LoadType.LoadFromBinary)
            {
                bytes = Utility.Encryption.GetXorBytes(bytes, hashBytes);
            }

            return ProcessOutput(platform, outputPackagePath, outputFullPath, additionalZipSelected, name, variant, resourceData, bytes, length, hashCode, compressedLength, compressedHashCode);
        }

        private bool ProcessBinary(Platform platform, string workingPath, string outputPackagePath, string outputFullPath, bool additionalZipSelected, string name, string variant)
        {
            string fullName = GetResourceFullName(name, variant);
            ResourceData resourceData = m_ResourceDatas[fullName];
            string assetName = resourceData.GetAssetNames()[0];
            string assetPath = Utility.Path.GetRegularPath(Application.dataPath.Substring(0, Application.dataPath.Length - AssetsStringLength) + assetName);

            byte[] bytes = File.ReadAllBytes(assetPath);
            int length = bytes.Length;
            int hashCode = Utility.Verifier.GetCrc32(bytes);
            int compressedLength = length;
            int compressedHashCode = hashCode;

            byte[] hashBytes = Utility.Converter.GetBytes(hashCode);
            if (resourceData.LoadType == LoadType.LoadFromMemory)
            {
                bytes = Utility.Encryption.GetQuickXorBytes(bytes, hashBytes);
            }
            else if (resourceData.LoadType == LoadType.LoadFromBinary)
            {
                bytes = Utility.Encryption.GetXorBytes(bytes, hashBytes);
            }

            return ProcessOutput(platform, outputPackagePath, outputFullPath, additionalZipSelected, name, variant, resourceData, bytes, length, hashCode, compressedLength, compressedHashCode);
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
                resources[index++] = new UpdatableVersionList.Resource(resourceData.Name, resourceData.Variant, GetExtension(resourceData), (byte)resourceData.LoadType, resourceCode.Length, resourceCode.HashCode, resourceCode.CompressedLength, resourceCode.CompressedHashCode, GetAssetIndexes(resourceData));
            }

            string[] fileSystemNames = GetFileSystemNames(resourceDatas);
            UpdatableVersionList.FileSystem[] fileSystems = new UpdatableVersionList.FileSystem[fileSystemNames.Length];
            for (int i = 0; i < fileSystems.Length; i++)
            {
                fileSystems[i] = new UpdatableVersionList.FileSystem(fileSystemNames[i], GetResourceIndexesFromFileSystem(resourceDatas, fileSystemNames[i]));
            }

            string[] resourceGroupNames = GetResourceGroupNames(resourceDatas);
            UpdatableVersionList.ResourceGroup[] resourceGroups = new UpdatableVersionList.ResourceGroup[resourceGroupNames.Length];
            for (int i = 0; i < resourceGroups.Length; i++)
            {
                resourceGroups[i] = new UpdatableVersionList.ResourceGroup(resourceGroupNames[i], GetResourceIndexesFromResourceGroup(resourceDatas, resourceGroupNames[i]));
            }

            UpdatableVersionList versionList = new UpdatableVersionList(ApplicableGameVersion, InternalResourceVersion, assets, resources, fileSystems, resourceGroups);
            UpdatableVersionListSerializer serializer = new UpdatableVersionListSerializer();
            string updatableVersionListPath = Utility.Path.GetRegularPath(Path.Combine(outputFullPath, Config.RemoteVersionListFileName));
            using (FileStream fileStream = new FileStream(updatableVersionListPath, FileMode.Create, FileAccess.Write))
            {
                if (!serializer.Serialize(fileStream, versionList))
                {
                    throw new EPloyException("Serialize updatable version list failure.");
                }
            }

            byte[] bytes = File.ReadAllBytes(updatableVersionListPath);
            int length = bytes.Length;
            int hashCode = Utility.Verifier.GetCrc32(bytes);
            bytes = Utility.Zip.Compress(bytes);
            int compressedLength = bytes.Length;
            File.WriteAllBytes(updatableVersionListPath, bytes);
            int compressedHashCode = Utility.Verifier.GetCrc32(bytes);
            int dotPosition = Config.RemoteVersionListFileName.LastIndexOf('.');
            string versionListFullNameWithCrc32 = Utility.Text.Format("{0}.{2:x8}.{1}", Config.RemoteVersionListFileName.Substring(0, dotPosition), Config.RemoteVersionListFileName.Substring(dotPosition + 1), hashCode);
            string updatableVersionListPathWithCrc32 = Utility.Path.GetRegularPath(Path.Combine(outputFullPath, versionListFullNameWithCrc32));
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
                assetIndexes[i] = Array.BinarySearch(assets, m_ResourceCollection.GetAsset(assetGuids[i]));
                if (assetIndexes[i] < 0)
                {
                    throw new EPloyException("Asset is invalid.");
                }
            }

            return assetIndexes;
        }

        private ResourceData[] GetPackedResourceDatas()
        {
            List<ResourceData> packedResourceDatas = new List<ResourceData>();
            foreach (ResourceData resourceData in m_ResourceDatas.Values)
            {
                if (!resourceData.Packed)
                {
                    continue;
                }

                packedResourceDatas.Add(resourceData);
            }

            return packedResourceDatas.ToArray();
        }

        private string[] GetFileSystemNames(IEnumerable<ResourceData> resourceDatas)
        {
            HashSet<string> fileSystemNames = new HashSet<string>();
            foreach (ResourceData resourceData in resourceDatas)
            {
                if (resourceData.FileSystem == null)
                {
                    continue;
                }

                fileSystemNames.Add(resourceData.FileSystem);
            }

            return fileSystemNames.OrderBy(x => x).ToArray();
        }

        private int[] GetResourceIndexesFromFileSystem(IEnumerable<ResourceData> resourceDatas, string fileSystemName)
        {
            int index = 0;
            List<int> resourceIndexes = new List<int>();
            foreach (ResourceData resourceData in resourceDatas)
            {
                if (resourceData.FileSystem == fileSystemName)
                {
                    resourceIndexes.Add(index);
                }

                index++;
            }

            resourceIndexes.Sort();
            return resourceIndexes.ToArray();
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

        private bool ProcessOutput(Platform platform, string outputPackagePath, string outputFullPath, bool additionalZipSelected, string name, string variant, ResourceData resourceData, byte[] bytes, int length, int hashCode, int compressedLength, int compressedHashCode)
        {
            string fullNameWithExtension = Utility.Text.Format("{0}.{1}", GetResourceFullName(name, variant), GetExtension(resourceData));

            if (OutputPackageSelected)
            {
                string packagePath = Utility.Path.GetRegularPath(Path.Combine(outputPackagePath, fullNameWithExtension));
                string packageDirectoryName = Path.GetDirectoryName(packagePath);
                if (!Directory.Exists(packageDirectoryName))
                {
                    Directory.CreateDirectory(packageDirectoryName);
                }
            }

            if (OutputFullSelected)
            {
                string fullNameWithCrc32AndExtension = variant != null ? Utility.Text.Format("{0}.{1}.{2:x8}.{3}", name, variant, hashCode, Config.DefaultExtension) : Utility.Text.Format("{0}.{1:x8}.{2}", name, hashCode, Config.DefaultExtension);
                string fullPath = Utility.Path.GetRegularPath(Path.Combine(outputFullPath, fullNameWithCrc32AndExtension));
                string fullDirectoryName = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(fullDirectoryName))
                {
                    Directory.CreateDirectory(fullDirectoryName);
                }

                if (additionalZipSelected)
                {
                    byte[] compressedBytes = Utility.Zip.Compress(bytes);
                    compressedLength = compressedBytes.Length;
                    compressedHashCode = Utility.Verifier.GetCrc32(compressedBytes);
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
                m_ResourceDatas.Add(resource.FullName, new ResourceData(resource.Name, resource.Variant, resource.FileSystem, resource.LoadType, resource.Packed, resource.GetResourceGroups()));
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
                int assetHashCode = Utility.Verifier.GetCrc32(assetBytes);

                List<string> dependencyAssetNames = new List<string>();
                DependencyData dependencyData = m_ResourceAnalyzerController.GetDependencyData(assetName);
                Asset[] dependencyAssets = dependencyData.GetDependencyAssets();
                foreach (Asset dependencyAsset in dependencyAssets)
                {
                    dependencyAssetNames.Add(dependencyAsset.Name);
                }

                dependencyAssetNames.Sort();

                m_ResourceDatas[asset.Resource.FullName].AddAssetData(asset.Guid, assetName, assetBytes.Length, assetHashCode, dependencyAssetNames.ToArray());
            }

            List<AssetBundleBuild> assetBundleBuildDataList = new List<AssetBundleBuild>();
            List<ResourceData> assetBundleResourceDataList = new List<ResourceData>();
            List<ResourceData> binaryResourceDataList = new List<ResourceData>();
            foreach (ResourceData resourceData in m_ResourceDatas.Values)
            {
                if (resourceData.AssetCount <= 0)
                {
                    m_BuildReport.LogError("Resource '{0}' has no asset.", GetResourceFullName(resourceData.Name, resourceData.Variant));
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
                    build.assetBundleVariant = resourceData.Variant;
                    build.assetNames = resourceData.GetAssetNames();
                    assetBundleBuildDataList.Add(build);
                }
            }

            assetBundleBuildDatas = assetBundleBuildDataList.ToArray();
            assetBundleResourceDatas = assetBundleResourceDataList.ToArray();
            binaryResourceDatas = binaryResourceDataList.ToArray();
            return true;
        }

        private static string GetResourceFullName(string name, string variant)
        {
            return !string.IsNullOrEmpty(variant) ? Utility.Text.Format("{0}.{1}", name, variant) : name;
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
                    throw new EPloyException("Platform is invalid.");
            }
        }

        private static string GetExtension(ResourceData data)
        {
            if (data.IsLoadFromBinary)
            {
                string assetName = data.GetAssetNames()[0];
                int position = assetName.LastIndexOf('.');
                if (position >= 0)
                {
                    return assetName.Substring(position + 1);
                }
            }

            return Config.DefaultExtension;
        }
    }
}
