using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DoorTarget", menuName = "DoorTarget")]
public class DoorTarget : ScriptableObject
{
    public string doorTitle = "[Door Title]";
    public string doorScene = "Scenes/Menu";
    public Color doorColor = new Color(152f/255f, 218f/255f, 243f/255f, 130f/255f);
    public bool keepTitleVisible = false;
    public bool locked = true;
    public bool startLocked = true;
    public bool getLocked { get {
        return locked && !GameManager._instance.winUnlocks.Contains(this);
    } }
    public DoorTarget[] destinations;
}
