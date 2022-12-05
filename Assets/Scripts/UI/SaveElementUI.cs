using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveElementUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _dateText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    public void Initiate(HorseSaveData saveData)
    {
        _nameText.text = saveData.Name;
        _dateText.text = saveData.Date.ToString("d");
        _descriptionText.text = saveData.Description;

    }
}
