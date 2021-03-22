using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EPloy.Editor.ResourceTools
{
    /// <summary>
    /// 资源生成器。
    /// </summary>
    internal sealed class ResourceBuilder : EditorWindow
    {
        private ResourceBuilderController m_Controller = null;
        private bool m_OrderBuildResources = false;

        [MenuItem("EPloy/ResTools/打包", false, 40)]
        private static void Open()
        {
            ResourceBuilder window = GetWindow<ResourceBuilder>("资源打包", true);
#if UNITY_2019_3_OR_NEWER
            window.minSize = new Vector2(800f, 640f);
#else
            window.minSize = new Vector2(800f, 600f);
#endif
        }

        private void OnEnable()
        {
            m_Controller = new ResourceBuilderController();
            m_Controller.OnLoadingResource += OnLoadingResource;
            m_Controller.OnLoadingAsset += OnLoadingAsset;
            m_Controller.OnLoadCompleted += OnLoadCompleted;
            m_Controller.OnAnalyzingAsset += OnAnalyzingAsset;
            m_Controller.OnAnalyzeCompleted += OnAnalyzeCompleted;
            m_Controller.ProcessingAssetBundle += OnProcessingAssetBundle;
            m_Controller.ProcessingBinary += OnProcessingBinary;
            m_Controller.ProcessResourceComplete += OnProcessResourceComplete;
            m_Controller.BuildResourceError += OnBuildResourceError;

            m_OrderBuildResources = false;

            if (m_Controller.Load())
            {
                Debug.Log("Load configuration success.");
                m_Controller.RefreshBuildEventHandler();
            }
            else
            {
                Debug.LogWarning("Load configuration failure.");
            }
        }

        private void Update()
        {
            if (m_OrderBuildResources)
            {
                m_OrderBuildResources = false;
                BuildResources();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width), GUILayout.Height(position.height));
            {
                GUILayout.Space(5f);
                EditorGUILayout.LabelField("环境信息", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Product Name", GUILayout.Width(160f));
                        EditorGUILayout.LabelField(m_Controller.ProductName);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Company Name", GUILayout.Width(160f));
                        EditorGUILayout.LabelField(m_Controller.CompanyName);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Game Identifier", GUILayout.Width(160f));
                        EditorGUILayout.LabelField(m_Controller.GameIdentifier);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("引用申明", GUILayout.Width(160f));
                        EditorGUILayout.LabelField(m_Controller.GameFrameworkVersion);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Unity Version", GUILayout.Width(160f));
                        EditorGUILayout.LabelField(m_Controller.UnityVersion);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Applicable Game Version", GUILayout.Width(160f));
                        EditorGUILayout.LabelField(m_Controller.ApplicableGameVersion);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Platforms", EditorStyles.boldLabel);
                        EditorGUILayout.BeginHorizontal("box");
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                DrawPlatform(Platform.Windows, "Windows");
                                DrawPlatform(Platform.Windows64, "Windows x64");
                                DrawPlatform(Platform.MacOS, "macOS");
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical();
                            {
                                DrawPlatform(Platform.Linux, "Linux");
                                DrawPlatform(Platform.IOS, "iOS");
                                DrawPlatform(Platform.Android, "Android");
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical();
                            {
                                DrawPlatform(Platform.WindowsStore, "Windows Store");
                                DrawPlatform(Platform.WebGL, "WebGL");
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5f);
                EditorGUILayout.LabelField("Zip", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("AssetBundle Zip", GUILayout.Width(160f));
                        m_Controller.AssetBundleZip = (AssetBundleZipType)EditorGUILayout.EnumPopup(m_Controller.AssetBundleZip);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Zip Helper : Utility.Zip", GUILayout.Width(160f));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);
                EditorGUILayout.LabelField("Build", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Force Rebuild AssetBundle", GUILayout.Width(160f));
                        m_Controller.ForceRebuildAssetBundleSelected = EditorGUILayout.Toggle(m_Controller.ForceRebuildAssetBundleSelected);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Internal Resource Version", GUILayout.Width(160f));
                        m_Controller.InternalResourceVersion = EditorGUILayout.IntField(m_Controller.InternalResourceVersion);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Resource Version", GUILayout.Width(160f));
                        GUILayout.Label(Utility.Text.Format("{0} ({1})", m_Controller.ApplicableGameVersion, m_Controller.InternalResourceVersion.ToString()));
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Output Directory", GUILayout.Width(160f));
                        m_Controller.OutputDirectory = EditorGUILayout.TextField(m_Controller.OutputDirectory);
                        if (GUILayout.Button("Browse...", GUILayout.Width(80f)))
                        {
                            BrowseOutputDirectory();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Working Path", GUILayout.Width(160f));
                        GUILayout.Label(m_Controller.WorkingPath);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUI.BeginDisabledGroup(!m_Controller.OutputPackageSelected);
                        EditorGUILayout.LabelField("Output Package Path", GUILayout.Width(160f));
                        GUILayout.Label(m_Controller.OutputPackagePath);
                        EditorGUI.EndDisabledGroup();
                        m_Controller.OutputPackageSelected = EditorGUILayout.ToggleLeft("Generate", m_Controller.OutputPackageSelected, GUILayout.Width(70f));
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUI.BeginDisabledGroup(!m_Controller.OutputFullSelected);
                        EditorGUILayout.LabelField("Output Full Path", GUILayout.Width(160f));
                        GUILayout.Label(m_Controller.OutputFullPath);
                        EditorGUI.EndDisabledGroup();
                        m_Controller.OutputFullSelected = EditorGUILayout.ToggleLeft("Generate", m_Controller.OutputFullSelected, GUILayout.Width(70f));
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUI.BeginDisabledGroup(!m_Controller.OutputPackedSelected);
                        EditorGUILayout.LabelField("Output Packed Path", GUILayout.Width(160f));
                        GUILayout.Label(m_Controller.OutputPackedPath);
                        EditorGUI.EndDisabledGroup();
                        m_Controller.OutputPackedSelected = EditorGUILayout.ToggleLeft("Generate", m_Controller.OutputPackedSelected, GUILayout.Width(70f));
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Build Report Path", GUILayout.Width(160f));
                        GUILayout.Label(m_Controller.BuildReportPath);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                string buildMessage = string.Empty;
                MessageType buildMessageType = MessageType.None;
                GetBuildMessage(out buildMessage, out buildMessageType);
                EditorGUILayout.HelpBox(buildMessage, buildMessageType);
                GUILayout.Space(2f);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginDisabledGroup(m_Controller.Platforms == Platform.Undefined || !m_Controller.IsValidOutputDirectory);
                    {
                        if (GUILayout.Button("Start Build Resources"))
                        {
                            m_OrderBuildResources = true;
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                    if (GUILayout.Button("Save", GUILayout.Width(80f)))
                    {
                        SaveConfiguration();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void BrowseOutputDirectory()
        {
            string directory = EditorUtility.OpenFolderPanel("Select Output Directory", m_Controller.OutputDirectory, string.Empty);
            if (!string.IsNullOrEmpty(directory))
            {
                m_Controller.OutputDirectory = directory;
            }
        }

        private void GetBuildMessage(out string message, out MessageType messageType)
        {
            message = string.Empty;
            messageType = MessageType.Error;
            if (m_Controller.Platforms == Platform.Undefined)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += Environment.NewLine;
                }

                message += "Platform is invalid.";
            }

            if (!m_Controller.IsValidOutputDirectory)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += Environment.NewLine;
                }

                message += "Output directory is invalid.";
            }

            if (!string.IsNullOrEmpty(message))
            {
                return;
            }

            messageType = MessageType.Info;
            if (Directory.Exists(m_Controller.OutputPackagePath))
            {
                message += Utility.Text.Format("{0} will be overwritten.", m_Controller.OutputPackagePath);
                messageType = MessageType.Warning;
            }

            if (Directory.Exists(m_Controller.OutputFullPath))
            {
                if (message.Length > 0)
                {
                    message += " ";
                }

                message += Utility.Text.Format("{0} will be overwritten.", m_Controller.OutputFullPath);
                messageType = MessageType.Warning;
            }

            if (Directory.Exists(m_Controller.OutputPackedPath))
            {
                if (message.Length > 0)
                {
                    message += " ";
                }

                message += Utility.Text.Format("{0} will be overwritten.", m_Controller.OutputPackedPath);
                messageType = MessageType.Warning;
            }

            if (messageType == MessageType.Warning)
            {
                return;
            }

            message = "Ready to build.";
        }

        private void BuildResources()
        {
            if (m_Controller.BuildResources())
            {
                Debug.Log("Build resources success.");
                SaveConfiguration();
            }
            else
            {
                Debug.LogWarning("Build resources failure.");
            }
        }

        private void SaveConfiguration()
        {
            if (m_Controller.Save())
            {
                Debug.Log("Save configuration success.");
            }
            else
            {
                Debug.LogWarning("Save configuration failure.");
            }
        }

        private void DrawPlatform(Platform platform, string platformName)
        {
            m_Controller.SelectPlatform(platform, EditorGUILayout.ToggleLeft(platformName, m_Controller.IsPlatformSelected(platform)));
        }

        private void OnLoadingResource(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Resources", Utility.Text.Format("Loading resources, {0}/{1} loaded.", index.ToString(), count.ToString()), (float)index / count);
        }

        private void OnLoadingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Assets", Utility.Text.Format("Loading assets, {0}/{1} loaded.", index.ToString(), count.ToString()), (float)index / count);
        }

        private void OnLoadCompleted()
        {
            EditorUtility.ClearProgressBar();
        }

        private void OnAnalyzingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Analyzing Assets", Utility.Text.Format("Analyzing assets, {0}/{1} analyzed.", index.ToString(), count.ToString()), (float)index / count);
        }

        private void OnAnalyzeCompleted()
        {
            EditorUtility.ClearProgressBar();
        }

        private bool OnProcessingAssetBundle(string assetBundleName, float progress)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Processing AssetBundle", Utility.Text.Format("Processing '{0}'...", assetBundleName), progress))
            {
                EditorUtility.ClearProgressBar();
                return true;
            }
            else
            {
                Repaint();
                return false;
            }
        }

        private bool OnProcessingBinary(string binaryName, float progress)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Processing Binary", Utility.Text.Format("Processing '{0}'...", binaryName), progress))
            {
                EditorUtility.ClearProgressBar();
                return true;
            }
            else
            {
                Repaint();
                return false;
            }
        }

        private void OnProcessResourceComplete(Platform platform)
        {
            EditorUtility.ClearProgressBar();
            Debug.Log(Utility.Text.Format("Build resources for '{0}' complete.", platform.ToString()));
        }

        private void OnBuildResourceError(string errorMessage)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogWarning(Utility.Text.Format("Build resources error with error message '{0}'.", errorMessage));
        }
    }
}
