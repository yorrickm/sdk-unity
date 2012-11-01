var upRotateSpeed : float;
var leftRotateSpeed : float;

function Awake()
{
	upRotateSpeed *= Mathf.Sign(Random.Range(-1,1));
	leftRotateSpeed *= Mathf.Sign(Random.Range(-1,1));
}

function Update ()
{
	//transform.RotateAround (Vector3.zero, Vector3.up, rotatespeed * Time.deltaTime);
 	transform.RotateAroundLocal (Vector3.up, upRotateSpeed * Time.deltaTime);
 	transform.RotateAroundLocal (Vector3.left, leftRotateSpeed * Time.deltaTime);
}

function OnBecameVisible()
{
	enabled = true;
}

function OnBecameInvisible()
{
	enabled = false;
}