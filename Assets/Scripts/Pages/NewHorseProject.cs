using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewHorseProject : Page
{
    [Header("Input fields")]
    [SerializeField] private TMP_InputField _horseNameInputField;
    [SerializeField] private TextMeshProUGUI _sexText;
    [SerializeField] private TMP_InputField _birthdayInputFiled;
    [SerializeField] private TMP_InputField _descriptionInputField;
    [SerializeField] private TMP_InputField _ownerNameInputFiled;
    [SerializeField] private TMP_InputField _phoneNumberInputField;
    [SerializeField] private TMP_InputField _localityInputFiled;

    [Space(10)]
    [Header("Buttons")]
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _cancelButton;

    [Space(10)]
    [SerializeField] private LoadScenePage _loadScenePage;

    private void Start()
    {
        _applyButton.onClick.AddListener(StartProject);
    }

    private void OnDestroy()
    {
        _applyButton.onClick.RemoveAllListeners();
    }

    private void StartProject()
    {
        string[] parseData = _birthdayInputFiled.text.Split('.');

        int day = int.Parse(parseData[0]);
        int month = int.Parse(parseData[1]);
        int year = int.Parse(parseData[2]);

        DateTime date = new DateTime(year, month, day);
        HorseData horse = new HorseData(
            _horseNameInputField.text,
            _sexText.text,
            date,
            _descriptionInputField.text,
            _ownerNameInputFiled.text,
            _localityInputFiled.text,
            new List<string>()
            );

        Storage storage = new Storage();
        storage.AddHorse(horse);

        SceneParameters.AddParam(horse);
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
        _loadScenePage.Open(loadingOperation);
    }
}
