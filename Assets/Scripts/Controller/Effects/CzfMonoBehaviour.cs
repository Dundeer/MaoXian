using UnityEngine;
using System.Collections;

public class CzfMonoBehaviour : MonoBehaviour
{
	//private Hashtable _comDict = new Hashtable();

	public T GetCom<T>()
	{
		//var k = typeof(T).ToString ();
		//if (!_comDict.ContainsKey (k)) {
		//	_comDict [k] = GetComponent<T> ();
		//}

		//return (T)_comDict [k];

        return GetComponent<T>();
	}
}