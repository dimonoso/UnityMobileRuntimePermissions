using MobileRuntimePermission;
using MobileRuntimePermission.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITestPermissions : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _logText;
	[SerializeField]
	private Button _checkCameraPermissionButton;
	[SerializeField]
	private Button _requestCameraPermissionButton;

	private void Start()
	{
		_checkCameraPermissionButton.onClick.AddListener(OnCheckCameraPermissionButton);
		_requestCameraPermissionButton.onClick.AddListener(OnRequestCameraPermissionButton);
	}

	private void OnCheckCameraPermissionButton()
	{
		var status = PermissionManager.PermissionStatus(Permission.CAMERA);

		_logText.text = $"PermissionStatus for {Permission.CAMERA}: {status}";
	}

	private void OnRequestCameraPermissionButton()
	{
		PermissionManager.RequestPermission(new PermissionData(Permission.CAMERA, () =>
		{
			_logText.text = $"PermissionRequest for {Permission.CAMERA} status: Allow";
		}, () =>
		{
			_logText.text = $"PermissionRequest for {Permission.CAMERA} status: Deny";
		}, () =>
		{
			_logText.text = $"PermissionRequest for {Permission.CAMERA} status: DenyNewerAskAgain";
		}));
	}
}
