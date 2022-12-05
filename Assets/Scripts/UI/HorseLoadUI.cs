using TMPro;
using UnityEngine;

public class HorseLoadUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _horseNameText;
    [SerializeField] private TextMeshProUGUI _ownerNameText;
    [SerializeField] private TextMeshProUGUI _localityText;

    public void Initiate(string horseName, string ownerName, string locality)
    {
        _horseNameText.text = horseName;
        _ownerNameText.text = ownerName;
        _localityText.text = locality;
    }

    public void Initiate(HorseData horse)
    {
        _horseNameText.text = horse.Name;
        _ownerNameText.text = horse.OwnerName;
        _localityText.text = horse.Locality;
    }
}
