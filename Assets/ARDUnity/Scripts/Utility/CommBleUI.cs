using UnityEngine;
using UnityEngine.UI;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/UI/CommBleUI")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/commbluetoothui")]
	public class CommBleUI : CommSocketUI
	{
		public CommBLE commBLE;
		public ListView deviceList;
		public ListItem deviceItem;
        public Button modify;
        public RectTransform settingDeviceName;
        public InputField inputDeviceName;
        public Button setNameOK;
        public Button setNameCancel;
        
        private bool _settingBLE = false;
        
	
		protected override void Awake()
		{
			base.Awake();
			
			commBLE.OnStartSearch.AddListener(OnStartSearch);
			commBLE.OnFoundDevice.AddListener(OnFoundDevice);
			commBLE.OnStopSearch.AddListener(OnStopSearch);
            commBLE.OnOpen.AddListener(OnBleOpen);
            commBLE.OnClose.AddListener(OnBleClose);
            commBLE.OnChangedDevice.AddListener(OnChangedDevice);
			
			deviceList.OnSelectionChanged.AddListener(OnSelectionChanged);
            modify.onClick.AddListener(OnShowSetDeviceName);
            setNameOK.onClick.AddListener(OnSetNameOK);
            setNameCancel.onClick.AddListener(OnSetNameCancel);
		}
		
		protected override void Start()
		{
			base.Start();
			
			deviceList.ClearItem();
            settingDeviceName.gameObject.SetActive(false);
		}
		
		public override void ShowUI()
		{
			base.ShowUI();
			
			commBLE.StartSearch();
		}
		
		protected override void CloseOK()
		{
			base.CloseOK();
			
			ListItem selectedItem = deviceList.selectedItem;
			if(selectedItem != null)
				commBLE.device = new CommDevice((CommDevice)selectedItem.data);
		}
		
		protected override void CloseCancel()
		{
			base.CloseCancel();
		}
		
		private void OnSelectionChanged(ListItem item)
		{
			if(item != null)
            {
                ok.interactable = true;
                modify.interactable = true;
            }
			else
            {
                ok.interactable = false;
                modify.interactable = false;
            }
		}
		
		private void OnStartSearch()
		{
			deviceList.ClearItem();
			ok.interactable = false;
            modify.interactable = false;
		}
		
		private void OnFoundDevice(CommDevice device)
		{
			ListItem item = GameObject.Instantiate(deviceItem);
			item.gameObject.SetActive(true);
			item.textList[0].text = device.name;
			if(item.textList.Length > 1)
				item.textList[1].text = device.address;
			item.data = device;
			
			deviceList.AddItem(item);
			
			if(deviceList.selectedItem == null)
			{
				if(commBLE.device.Equals(device))
					deviceList.selectedItem = item;
			}			
		}
		
		private void OnStopSearch()
		{
		}
        
        private void OnBleOpen()
        {
            if(_settingBLE)
            {
                settingCommSocket.gameObject.SetActive(false);
                settingDeviceName.gameObject.SetActive(true);
                inputDeviceName.text = commBLE.device.name;
            }
        }
        
        private void OnBleClose()
        {
            if(_settingBLE)
            {
                _settingBLE = false;
                ShowUI();
            }
        }
        
        private void OnChangedDevice(CommDevice device)
        {
            commBLE.Close();
        }
        
        private void OnShowSetDeviceName()
        {
            _settingBLE = true;
  
            ListItem selectedItem = deviceList.selectedItem;
			if(selectedItem != null)
            {
                commBLE.device = new CommDevice((CommDevice)selectedItem.data);
                commBLE.Open();
            }
        }
        
        private void OnSetNameOK()
        {
			settingDeviceName.gameObject.SetActive(false);
            commBLE.SetDeviceName(inputDeviceName.text);
        }
        
        private void OnSetNameCancel()
        {
			settingDeviceName.gameObject.SetActive(false);
            commBLE.Close();
        }
	}
}
