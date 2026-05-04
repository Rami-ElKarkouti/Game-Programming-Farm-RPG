using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimalProductionManager : MonoBehaviour
{
    [SerializeField] private AnimalController[] _animals;
    [SerializeField] private TextMeshProUGUI _productionLogText;

    private Dictionary<string, int> _productionTotals = new Dictionary<string, int>();

    public void AdvanceDayAndProduce()
    {
        string dailyLog = "Today:\n";

        for (int i = 0; i < _animals.Length; i++)
        {
            if (_animals[i] == null)
            {
                continue;
            }

            _animals[i].AdvanceDay();

            if (_animals[i].CanProduce())
            {
                string item = _animals[i].GetProductionItem();

                if (!_productionTotals.ContainsKey(item))
                {
                    _productionTotals[item] = 0;
                }

                _productionTotals[item]++;

                dailyLog += _animals[i].GetAnimalName() + " produced " + item + "\n";
            }
            else
            {
                dailyLog += _animals[i].GetAnimalName() + " was too hungry to produce\n";
            }
        }

        UpdateProductionLogUI(dailyLog);
        Debug.Log(dailyLog);
    }

    private void UpdateProductionLogUI(string dailyLog)
    {
        if (_productionLogText == null)
        {
            return;
        }

        string totalsText = "\nTotals:\n";

        foreach (KeyValuePair<string, int> entry in _productionTotals)
        {
            totalsText += entry.Key + ": " + entry.Value + "\n";
        }

        _productionLogText.text = dailyLog + totalsText;
    }
}