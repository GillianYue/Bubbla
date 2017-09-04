struct ClassRegistrationContext;
void InvokeRegisterStaticallyLinkedModuleClasses(ClassRegistrationContext& context)
{
	// Do nothing (we're in stripping mode)
}

void RegisterStaticallyLinkedModulesGranular()
{
	void RegisterModule_Animation();
	RegisterModule_Animation();

	void RegisterModule_Audio();
	RegisterModule_Audio();

	void RegisterModule_CloudWebServices();
	RegisterModule_CloudWebServices();

	void RegisterModule_ParticleSystem();
	RegisterModule_ParticleSystem();

	void RegisterModule_Physics();
	RegisterModule_Physics();

	void RegisterModule_Physics2D();
	RegisterModule_Physics2D();

	void RegisterModule_TextRendering();
	RegisterModule_TextRendering();

	void RegisterModule_UI();
	RegisterModule_UI();

	void RegisterModule_UnityConnect();
	RegisterModule_UnityConnect();

	void RegisterModule_IMGUI();
	RegisterModule_IMGUI();

	void RegisterModule_UnityWebRequest();
	RegisterModule_UnityWebRequest();

}

void RegisterAllClasses()
{
	//Total: 81 classes
	//0. Renderer
	void RegisterClass_Renderer();
	RegisterClass_Renderer();

	//1. Component
	void RegisterClass_Component();
	RegisterClass_Component();

	//2. EditorExtension
	void RegisterClass_EditorExtension();
	RegisterClass_EditorExtension();

	//3. GUILayer
	void RegisterClass_GUILayer();
	RegisterClass_GUILayer();

	//4. Behaviour
	void RegisterClass_Behaviour();
	RegisterClass_Behaviour();

	//5. Texture
	void RegisterClass_Texture();
	RegisterClass_Texture();

	//6. NamedObject
	void RegisterClass_NamedObject();
	RegisterClass_NamedObject();

	//7. Texture2D
	void RegisterClass_Texture2D();
	RegisterClass_Texture2D();

	//8. RenderTexture
	void RegisterClass_RenderTexture();
	RegisterClass_RenderTexture();

	//9. Mesh
	void RegisterClass_Mesh();
	RegisterClass_Mesh();

	//10. NetworkView
	void RegisterClass_NetworkView();
	RegisterClass_NetworkView();

	//11. RectTransform
	void RegisterClass_RectTransform();
	RegisterClass_RectTransform();

	//12. Transform
	void RegisterClass_Transform();
	RegisterClass_Transform();

	//13. Shader
	void RegisterClass_Shader();
	RegisterClass_Shader();

	//14. TextAsset
	void RegisterClass_TextAsset();
	RegisterClass_TextAsset();

	//15. Material
	void RegisterClass_Material();
	RegisterClass_Material();

	//16. Sprite
	void RegisterClass_Sprite();
	RegisterClass_Sprite();

	//17. SpriteRenderer
	void RegisterClass_SpriteRenderer();
	RegisterClass_SpriteRenderer();

	//18. Camera
	void RegisterClass_Camera();
	RegisterClass_Camera();

	//19. MonoBehaviour
	void RegisterClass_MonoBehaviour();
	RegisterClass_MonoBehaviour();

	//20. GameObject
	void RegisterClass_GameObject();
	RegisterClass_GameObject();

	//21. ParticleSystem
	void RegisterClass_ParticleSystem();
	RegisterClass_ParticleSystem();

	//22. Rigidbody
	void RegisterClass_Rigidbody();
	RegisterClass_Rigidbody();

	//23. Collider
	void RegisterClass_Collider();
	RegisterClass_Collider();

	//24. Collider2D
	void RegisterClass_Collider2D();
	RegisterClass_Collider2D();

	//25. AudioClip
	void RegisterClass_AudioClip();
	RegisterClass_AudioClip();

	//26. SampleClip
	void RegisterClass_SampleClip();
	RegisterClass_SampleClip();

	//27. AudioSource
	void RegisterClass_AudioSource();
	RegisterClass_AudioSource();

	//28. AudioBehaviour
	void RegisterClass_AudioBehaviour();
	RegisterClass_AudioBehaviour();

	//29. Animator
	void RegisterClass_Animator();
	RegisterClass_Animator();

	//30. DirectorPlayer
	void RegisterClass_DirectorPlayer();
	RegisterClass_DirectorPlayer();

	//31. Font
	void RegisterClass_Font();
	RegisterClass_Font();

	//32. Canvas
	void RegisterClass_Canvas();
	RegisterClass_Canvas();

	//33. CanvasGroup
	void RegisterClass_CanvasGroup();
	RegisterClass_CanvasGroup();

	//34. CanvasRenderer
	void RegisterClass_CanvasRenderer();
	RegisterClass_CanvasRenderer();

	//35. RuntimeAnimatorController
	void RegisterClass_RuntimeAnimatorController();
	RegisterClass_RuntimeAnimatorController();

	//36. PreloadData
	void RegisterClass_PreloadData();
	RegisterClass_PreloadData();

	//37. Cubemap
	void RegisterClass_Cubemap();
	RegisterClass_Cubemap();

	//38. Texture3D
	void RegisterClass_Texture3D();
	RegisterClass_Texture3D();

	//39. Texture2DArray
	void RegisterClass_Texture2DArray();
	RegisterClass_Texture2DArray();

	//40. LevelGameManager
	void RegisterClass_LevelGameManager();
	RegisterClass_LevelGameManager();

	//41. GameManager
	void RegisterClass_GameManager();
	RegisterClass_GameManager();

	//42. TimeManager
	void RegisterClass_TimeManager();
	RegisterClass_TimeManager();

	//43. GlobalGameManager
	void RegisterClass_GlobalGameManager();
	RegisterClass_GlobalGameManager();

	//44. AudioManager
	void RegisterClass_AudioManager();
	RegisterClass_AudioManager();

	//45. InputManager
	void RegisterClass_InputManager();
	RegisterClass_InputManager();

	//46. Physics2DSettings
	void RegisterClass_Physics2DSettings();
	RegisterClass_Physics2DSettings();

	//47. MeshRenderer
	void RegisterClass_MeshRenderer();
	RegisterClass_MeshRenderer();

	//48. GraphicsSettings
	void RegisterClass_GraphicsSettings();
	RegisterClass_GraphicsSettings();

	//49. MeshFilter
	void RegisterClass_MeshFilter();
	RegisterClass_MeshFilter();

	//50. QualitySettings
	void RegisterClass_QualitySettings();
	RegisterClass_QualitySettings();

	//51. PhysicsManager
	void RegisterClass_PhysicsManager();
	RegisterClass_PhysicsManager();

	//52. MeshCollider
	void RegisterClass_MeshCollider();
	RegisterClass_MeshCollider();

	//53. BoxCollider
	void RegisterClass_BoxCollider();
	RegisterClass_BoxCollider();

	//54. AnimationClip
	void RegisterClass_AnimationClip();
	RegisterClass_AnimationClip();

	//55. Motion
	void RegisterClass_Motion();
	RegisterClass_Motion();

	//56. TagManager
	void RegisterClass_TagManager();
	RegisterClass_TagManager();

	//57. AudioListener
	void RegisterClass_AudioListener();
	RegisterClass_AudioListener();

	//58. AnimatorController
	void RegisterClass_AnimatorController();
	RegisterClass_AnimatorController();

	//59. ScriptMapper
	void RegisterClass_ScriptMapper();
	RegisterClass_ScriptMapper();

	//60. TrailRenderer
	void RegisterClass_TrailRenderer();
	RegisterClass_TrailRenderer();

	//61. DelayedCallManager
	void RegisterClass_DelayedCallManager();
	RegisterClass_DelayedCallManager();

	//62. RenderSettings
	void RegisterClass_RenderSettings();
	RegisterClass_RenderSettings();

	//63. Light
	void RegisterClass_Light();
	RegisterClass_Light();

	//64. MonoScript
	void RegisterClass_MonoScript();
	RegisterClass_MonoScript();

	//65. MonoManager
	void RegisterClass_MonoManager();
	RegisterClass_MonoManager();

	//66. FlareLayer
	void RegisterClass_FlareLayer();
	RegisterClass_FlareLayer();

	//67. PlayerSettings
	void RegisterClass_PlayerSettings();
	RegisterClass_PlayerSettings();

	//68. PhysicMaterial
	void RegisterClass_PhysicMaterial();
	RegisterClass_PhysicMaterial();

	//69. SphereCollider
	void RegisterClass_SphereCollider();
	RegisterClass_SphereCollider();

	//70. CapsuleCollider
	void RegisterClass_CapsuleCollider();
	RegisterClass_CapsuleCollider();

	//71. BuildSettings
	void RegisterClass_BuildSettings();
	RegisterClass_BuildSettings();

	//72. ResourceManager
	void RegisterClass_ResourceManager();
	RegisterClass_ResourceManager();

	//73. NetworkManager
	void RegisterClass_NetworkManager();
	RegisterClass_NetworkManager();

	//74. MasterServerInterface
	void RegisterClass_MasterServerInterface();
	RegisterClass_MasterServerInterface();

	//75. LightmapSettings
	void RegisterClass_LightmapSettings();
	RegisterClass_LightmapSettings();

	//76. ParticleSystemRenderer
	void RegisterClass_ParticleSystemRenderer();
	RegisterClass_ParticleSystemRenderer();

	//77. LightProbes
	void RegisterClass_LightProbes();
	RegisterClass_LightProbes();

	//78. RuntimeInitializeOnLoadManager
	void RegisterClass_RuntimeInitializeOnLoadManager();
	RegisterClass_RuntimeInitializeOnLoadManager();

	//79. CloudWebServicesManager
	void RegisterClass_CloudWebServicesManager();
	RegisterClass_CloudWebServicesManager();

	//80. UnityConnectSettings
	void RegisterClass_UnityConnectSettings();
	RegisterClass_UnityConnectSettings();

}
