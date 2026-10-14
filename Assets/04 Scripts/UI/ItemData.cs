using UnityEngine;
[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData", order = 5)]
public class ItemData : ScriptableObject
{
  public string Name;
  public string Description;
  public Sprite Icon;
  public string lore;
  public int emplacement;
}
