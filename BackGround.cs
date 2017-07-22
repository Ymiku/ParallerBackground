using UnityEngine;
using System.Collections;

public class BackGround : MonoBehaviour {
	public bool isSerial = false;
	public bool adjustScale = true;
	public bool is3DView = false;
	public float relativeSpeed = 0.8f;
	private Vector2 _size = Vector2.zero;
	private float _width;
	//only if isSerial==false
	private float _startBX;
	private float _startX;
	private float _endX;
	//only if isSerial==true
	public float interval;
	private int _arrayStartPos;
	private float _lastX;
	private Transform[] backs;
	private Transform cameraTrans;
	// Use this for initialization
	void Start () {
		cameraTrans = BackGroundManager.Instance.mainCamera.transform;
		AdjustScale ();
		GetSize ();
		Clone ();
		Calculate ();
	}
	void AdjustScale()
	{
		if (!adjustScale)
			return;
		float sizeTemp;
		Vector2 size = Vector2.zero;
		SpriteRenderer[] rendererArray =  GetComponentsInChildren<SpriteRenderer>(); 
		if (rendererArray.Length == 0) 
			return;
		for (int i = 0; i < rendererArray.Length; i++) {
			sizeTemp = -rendererArray [i].bounds.size.y/2f + rendererArray [i].transform.position.y;
			if (sizeTemp < size.x||i==0) {
				size = new Vector2 (sizeTemp,size.y);
			}
			sizeTemp += rendererArray [i].bounds.size.y;
			if (sizeTemp > size.y||i==0) {
				size = new Vector2 (size.x,sizeTemp);
			}
		}
		size -= new Vector2 (transform.position.y,transform.position.y);
		if(!is3DView)
		{
			float height = size.y - size.x;
			transform.position -= new Vector3 (0f,height*(1f-relativeSpeed)*0.5f,0f);
		}
		transform.localScale = new Vector3 (transform.localScale.x*relativeSpeed,transform.localScale.y*relativeSpeed,1f);

	}
	void GetSize()
	{
		float sizeTemp;
		SpriteRenderer[] rendererArray =  GetComponentsInChildren<SpriteRenderer>(); 
		if (rendererArray.Length == 0) 
			return;
		for (int i = 0; i < rendererArray.Length; i++) {
			sizeTemp = -rendererArray [i].bounds.size.x/2f + rendererArray [i].transform.position.x;
			if (sizeTemp < _size.x||i==0) {
				_size = new Vector2 (sizeTemp,_size.y);
			}
			sizeTemp += rendererArray [i].bounds.size.x;
			if (sizeTemp > _size.y||i==0) {
				_size = new Vector2 (_size.x,sizeTemp);
			}
		}
		_size -= new Vector2 (transform.position.x,transform.position.x);
		_width = _size.y - _size.x;
	}
	void Clone()
	{
		if (!isSerial)
			return;
		int num = (int)(BackGroundManager.Instance.cameraWidth/(_width+interval));
		num++;
		backs = new Transform[num+1]; 
		backs [0] = transform;
		for (int i = 0; i < num; i++) {
			GameObject t = BackGroundManager.Instance.CloneBackGround (gameObject);
			backs [i + 1] = t.transform;
		}
		SetOriPos ();
	}
	void SetOriPos()
	{
		if (!isSerial)
		return;
		int eachNum = (int)(backs.Length/2f);
		float intX;
		int backCount = 0;
		if (backs.Length % 2f == 0) {
			intX = cameraTrans.position.x + _width/2f - (interval + _width)*eachNum;
			for (int i = 0; i < backs.Length; i++) {
				backs [backCount].position = new Vector3 (intX,backs [backCount].position.y,backs [backCount].position.z);
				intX += (interval + _width);
				backCount++;
			}
		} else {
			intX = cameraTrans.position.x - interval/2f - (interval + _width)*eachNum;
			backCount = 0;
			for (int i = 0; i < backs.Length; i++) {
				backs [backCount].position = new Vector3 (intX,backs [backCount].position.y,backs [backCount].position.z);
				intX += (interval + _width);
				backCount++;
			}
		}
		_arrayStartPos = 0;
		_lastX = cameraTrans.position.x;
	}
	void Calculate()
	{
		if (!isSerial) {
			float dis = _width / 2f + BackGroundManager.Instance.cameraHalfWidth;
			_startBX = transform.position.x - dis / relativeSpeed + dis;
			_startX = transform.position.x - dis / relativeSpeed;
			_endX = transform.position.x + dis / relativeSpeed;
			return;
		}

	}
	// Update is called once per frame
	void Update () {
		if (isSerial) {
			SerialUpdate ();
		} else {
			UnitUpdate ();
		}
	}
	void SerialUpdate()
	{
		float dx = cameraTrans.position.x - _lastX;
		for (int i = 0; i < backs.Length; i++) {
			backs [i].position += new Vector3 (dx*(1f-relativeSpeed),0f,0f);
		}

		_lastX = cameraTrans.position.x;
		FitScreen ();
	}
	void FitScreen()
	{
		while (true) {
			if (backs [_arrayStartPos].position.x + _width / 2f + interval < cameraTrans.position.x - BackGroundManager.Instance.cameraHalfWidth) {
				backs [_arrayStartPos].position += new Vector3 (backs.Length*(_width+interval),0f,0f);
				StartPosAddOne ();
			} else {
				break;
			}
		}
		while (true) {
			if (backs [GetEndPos()].position.x - _width / 2f > cameraTrans.position.x + BackGroundManager.Instance.cameraHalfWidth) {
				backs [GetEndPos()].position -= new Vector3 (backs.Length*(_width+interval),0f,0f);
				StartPosReduceOne ();
			} else {
				break;
			}
		}
	}
	void StartPosAddOne()
	{
		_arrayStartPos++;
		if (_arrayStartPos >= backs.Length) {
			_arrayStartPos = 0;
		}
	}
	void StartPosReduceOne()
	{
		_arrayStartPos--;
		if (_arrayStartPos <= -1) {
			_arrayStartPos = backs.Length-1;
		}
	}
	int GetEndPos()
	{
		if (_arrayStartPos == 0)
			return backs.Length - 1;
		return _arrayStartPos - 1;
	}
	void UnitUpdate()
	{
		if (cameraTrans.position.x >= _startX && cameraTrans.position.x <= _endX) {
			transform.position = new Vector3 (_startBX+(cameraTrans.position.x-_startX)*(1f-relativeSpeed),transform.position.y,transform.position.z);
		}
	}
}
