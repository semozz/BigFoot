using UnityEngine;
using System.Collections;

public class Joystic : MonoBehaviour {
	public UISlider centerButton = null;
	public Collider leftArea = null;
	public Collider rightArea = null;
	
	public PlayerController player = null;
	
	// Use this for initialization
	void Start () {
		InitInfos();
	}
	
	public void InitInfos()
	{
		if  (centerButton != null)
		{
			centerButton.resetValue = 0.5f;
			centerButton.sliderValue = 0.5f;
			
			if (centerButton.thumb != null)
			{
				UIEventListener listener = UIEventListener.Get(centerButton.thumb.gameObject);
				listener.onPress += OnPressThumb;
				//listener.onDrag += OnDragThumb;
			}
		}
	}
	
	
	// Update is called once per frame
	void Update () {
		if (player == null || player.IsAliveState() == false)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			if (centerButton != null)
				centerButton.sliderValue = 0.5f;
			
			player.input.ResetInput();
			
			player.input.AddInputs (PlayerInput.eControlKey.CK_RIGHT, false);
			player.input.AddInputs (PlayerInput.eControlKey.CK_LEFT, false);
			return;
		}
		
		if (player.input.mMoveKey.x == 1.0f)
			player.input.AddInputs (PlayerInput.eControlKey.CK_RIGHT, true);
		else if (player.input.mMoveKey.x == -1.0f)
			player.input.AddInputs (PlayerInput.eControlKey.CK_LEFT, true);
		else
		{
			player.input.AddInputs (PlayerInput.eControlKey.CK_RIGHT, false);
			player.input.AddInputs (PlayerInput.eControlKey.CK_LEFT, false);
		}
	}
	
	public void OnLeftButtonPress()
	{
		if (player == null || player.IsAliveState() == false)
			return;
		
		player.input.mMoveKey.x = -1.0f;
		if (centerButton != null)
			centerButton.sliderValue = 0.0f;
		//player.input.AddInputs (PlayerInput.eControlKey.CK_LEFT, true);
	}
	
	public void OnLeftButtonRelease()
	{
		if (player == null)
			return;
		
		player.input.mMoveKey.x = 0.0f;
		if (centerButton != null)
			centerButton.sliderValue = 0.5f;
		//player.input.AddInputs (PlayerInput.eControlKey.CK_LEFT, false);
	}
	
	public void OnRightButtonPress()
	{
		if (player == null || player.IsAliveState() == false)
			return;
		
		player.input.mMoveKey.x = 1.0f;
		if (centerButton != null)
		{
			centerButton.sliderValue = 1.0f;
		
			/*
			if (centerButton.thumb != null)
			{
				UIEventListener listener = UIEventListener.Get(centerButton.thumb.gameObject);
				if (listener.onPress != null)
					listener.onPress(centerButton.thumb.gameObject, true);
			}
			*/
		}
		//player.input.AddInputs (PlayerInput.eControlKey.CK_RIGHT, true);
	}
	
	public void OnRightButtonRelease()
	{
		if (player == null)
			return;
		
		player.input.mMoveKey.x = 0.0f;
		if (centerButton != null)
		{
			centerButton.sliderValue = 0.5f;
			
			/*
			if (centerButton.thumb != null)
			{
				UIEventListener listener = UIEventListener.Get(centerButton.thumb.gameObject);
				if (listener.onPress != null)
					listener.onPress(centerButton.thumb.gameObject, false);
			}
			*/
		}
		//player.input.AddInputs (PlayerInput.eControlKey.CK_RIGHT, false);
	}
	
	void OnPressThumb (GameObject go, bool pressed)
	{
		if (pressed == false)
		{
			player.input.mMoveKey.x = 0.0f;
			
			centerButton.sliderValue = 0.5f;
		}
	}
	
	public void OnSliderChange()
	{
		if (centerButton.thumb != null)
		{
			Vector3 pos = centerButton.thumb.position;
			
			float moveDir = 0.0f;
			if (leftArea != null && rightArea != null)
			{
				if (leftArea.bounds.Contains(pos) == true)
					moveDir = -1.0f;
				else if (this.rightArea.bounds.Contains(pos) == true)
					moveDir = 1.0f;
			}
			
			if (player != null)
				player.input.mMoveKey.x = moveDir;
			
			//centerButton.sliderValue = 0.5f;
		}
	}
}
