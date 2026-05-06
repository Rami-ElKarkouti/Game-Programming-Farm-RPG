using TMPro;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string _animalName = "Chicken 01";
    [SerializeField] private string _animalType = "Chicken";

    [Header("Stats")]
    [SerializeField] private int _maxHunger = 10;
    [SerializeField] private int _currentHunger = 3;

    [SerializeField] private int _maxHappiness = 10;
    [SerializeField] private int _currentHappiness = 5;

    [Header("UI")]
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private TextMeshPro _stateText;

    private void Start()
    {
        UpdateAnimalUI();
    }

    public void FeedAnimal()
    {
        _currentHunger = 0;
        UpdateAnimalUI();
        Debug.Log(_animalName + " was fed.");
    }

    public void PetAnimal()
    {
        _currentHappiness++;
        _currentHappiness = Mathf.Clamp(_currentHappiness, 0, _maxHappiness);
        UpdateAnimalUI();
        Debug.Log(_animalName + " was petted.");
    }

public void AdvanceDay()
{
    // Each new day makes the animal more hungry.
    _currentHunger++;
    _currentHunger = Mathf.Clamp(_currentHunger, 0, _maxHunger);

    // Only reduce happiness once hunger reaches 30 or more.
    if (_currentHunger >= 30)
    {
        _currentHappiness--;
        _currentHappiness = Mathf.Clamp(_currentHappiness, 0, _maxHappiness);
    }

    Debug.Log(_animalName + " AdvanceDay called. Hunger = " + _currentHunger + ", Happiness = " + _currentHappiness);

    UpdateAnimalUI();
}

    public bool CanProduce()
    {
        return _currentHunger < 8;
    }

    public string GetProductionItem()
    {
        if (_animalType == "Chicken")
        {
            return "Egg";
        }

        if (_animalType == "Cow")
        {
            return "Milk";
        }

        if (_animalType == "Sheep")
        {
            return "Wool";
        }

        return "Unknown";
    }

    public string GetAnimalName()
    {
        return _animalName;
    }

    public string GetAnimalType()
    {
        return _animalType;
    }

    public int GetCurrentHunger()
    {
        return _currentHunger;
    }

    public int GetCurrentHappiness()
    {
        return _currentHappiness;
    }

    public bool IsProductionReady()
    {
        return CanProduce();
    }

    private void UpdateAnimalUI()
    {
        if (_nameText != null)
        {
            _nameText.text = _animalName;
        }

        if (_stateText != null)
        {
            if (_currentHunger >= 8 && _currentHappiness <= 3)
            {
                _stateText.text = "Sad";
            }
            else if (_currentHunger >= 8)
            {
                _stateText.text = "Hungry";
            }
            else if (_currentHappiness >= 8)
            {
                _stateText.text = "Happy";
            }
            else if (_currentHunger == 0)
            {
                _stateText.text = "Fed";
            }
            else
            {
                _stateText.text = "Normal";
            }
        }
    }


    public string GetSaveId()
    {
        return gameObject.name;
    }

    public AnimalSaveData GetSaveData()
    {
        AnimalSaveData data = new AnimalSaveData();
        data.AnimalId = GetSaveId();
        data.AnimalName = _animalName;
        data.AnimalType = _animalType;
        data.CurrentHunger = _currentHunger;
        data.CurrentHappiness = _currentHappiness;
        return data;
    }

    public void LoadSaveData(AnimalSaveData data)
    {
        if (data == null)
        {
            return;
        }

        _currentHunger = Mathf.Clamp(data.CurrentHunger, 0, _maxHunger);
        _currentHappiness = Mathf.Clamp(data.CurrentHappiness, 0, _maxHappiness);
        UpdateAnimalUI();
    }
}