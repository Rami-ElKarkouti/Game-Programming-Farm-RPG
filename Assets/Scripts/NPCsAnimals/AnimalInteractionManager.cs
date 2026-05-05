using TMPro;
using UnityEngine;

public class AnimalInteractionManager : MonoBehaviour
{
    [SerializeField] private AnimalController[] _animals;
    [SerializeField] private TextMeshProUGUI _selectedAnimalText;
    [SerializeField] private TextMeshProUGUI _feedbackText;

    private int _selectedAnimalIndex = 0;

    private void Start()
    {
        UpdateSelectedAnimalUI();
        ClearFeedback();
    }

    private void Update()
    {
        HandleAnimalSelection();
        HandleAnimalInteraction();
    }

    private void HandleAnimalSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectAnimal(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectAnimal(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectAnimal(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectAnimal(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectAnimal(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectAnimal(5);
        }
    }

    private void HandleAnimalInteraction()
    {
        AnimalController selectedAnimal = GetSelectedAnimalInternal();

        if (selectedAnimal == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            selectedAnimal.FeedAnimal();
            ShowFeedback("Fed " + selectedAnimal.GetAnimalName());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            selectedAnimal.PetAnimal();
            ShowFeedback("Petted " + selectedAnimal.GetAnimalName());
        }
    }

    private void SelectAnimal(int index)
    {
        if (_animals == null || index < 0 || index >= _animals.Length || _animals[index] == null)
        {
            return;
        }

        _selectedAnimalIndex = index;
        UpdateSelectedAnimalUI();
        ShowFeedback("Selected " + _animals[index].GetAnimalName());
    }

    public AnimalController GetSelectedAnimal()
    {
        return GetSelectedAnimalInternal();
    }

    private AnimalController GetSelectedAnimalInternal()
    {
        if (_animals == null || _selectedAnimalIndex < 0 || _selectedAnimalIndex >= _animals.Length)
        {
            return null;
        }

        return _animals[_selectedAnimalIndex];
    }

    private void UpdateSelectedAnimalUI()
    {
        if (_selectedAnimalText == null)
        {
            return;
        }

        AnimalController selectedAnimal = GetSelectedAnimalInternal();

        if (selectedAnimal == null)
        {
            _selectedAnimalText.text = "Selected Animal: None";
            return;
        }

        _selectedAnimalText.text = "Selected Animal: " + selectedAnimal.GetAnimalName();
    }

    private void ShowFeedback(string message)
    {
        if (_feedbackText != null)
        {
            _feedbackText.text = message;
        }

        Debug.Log(message);
    }

    private void ClearFeedback()
    {
        if (_feedbackText != null)
        {
            _feedbackText.text = "";
        }
    }


    public AnimalInteractionSaveData GetSaveData()
    {
        AnimalInteractionSaveData data = new AnimalInteractionSaveData();
        data.SelectedAnimalIndex = _selectedAnimalIndex;
        return data;
    }

    public void LoadSaveData(AnimalInteractionSaveData data)
    {
        if (data == null)
        {
            return;
        }

        if (_animals == null || _animals.Length == 0)
        {
            _selectedAnimalIndex = 0;
        }
        else
        {
            _selectedAnimalIndex = Mathf.Clamp(data.SelectedAnimalIndex, 0, _animals.Length - 1);
        }

        UpdateSelectedAnimalUI();
        ClearFeedback();
    }
}