using System;
using UnityEngine;
using UnityEngine.UI;

public class AnimalNextSpawn : MonoBehaviour
{
    public Image imgAnimal;
    public Text textAnimal;
    public Image imgAnimalName;
    public Text textLT;
    public Image imgLT;
    public Text textRT;
    public Image imgRT;

    DateTime spawnTime;
    DateTime hideTime;
    DateTime nextHideTime;

    bool isSpawned;

    AnimalData animalData;

    public void SetAnimal(int index) => imgAnimal.sprite = Resources.Load<Sprite>($"Sprite/Animal/{index}");

    public void SetAnimalName(int index) => textAnimal.text = ResourceManager.instance.GetAnimalName(index);

    public void SetData(AnimalData data) => animalData = data;

    public void SetDefaultTime(DateTime _spawn, DateTime _hide, DateTime _next)
    {
        spawnTime = _spawn;
        hideTime = _hide;
        nextHideTime = _next;
        if (hideTime != new DateTime(1970, 1, 1) && hideTime < spawnTime)
        {
            isSpawned = true;
            textLT.text = hideTime.ToString("MM-dd HH:mm:ss");
            imgAnimalName.color = Color.yellow;
            imgLT.color = Color.yellow;
            imgRT.color = Color.yellow;
            animalData.SetPopUp(true);
        }
        else
        {
            isSpawned = false;
            textLT.text = spawnTime.ToString("MM-dd HH:mm:ss");
            imgAnimalName.color = Color.white;
            imgLT.color = Color.white;
            imgRT.color = Color.white;
            animalData.SetPopUp(false);
        }
    }

    public void SetSpawnTime(DateTime _spawnTime, DateTime _hideTime)
    {
        isSpawned = true;
        spawnTime = _spawnTime;
        nextHideTime = _hideTime;
        textLT.text = hideTime.ToString("MM-dd HH:mm:ss");
        imgAnimalName.color = Color.yellow;
        imgLT.color = Color.yellow;
        imgRT.color = Color.yellow;
        animalData.SetPopUp(true);
    }

    public void SetHideTime()
    {
        isSpawned = false;
        hideTime = nextHideTime;
        textLT.text = spawnTime.ToString("MM-dd HH:mm:ss");
        imgAnimalName.color = Color.white;
        imgLT.color = Color.white;
        imgRT.color = Color.white;
        animalData.SetPopUp(false);
    }

    public DateTime GetSpawnTime() => spawnTime;

    public DateTime GetHideTime() => hideTime;

    private void Update()
    {
        if (isSpawned)
            textRT.text = (hideTime.ToUniversalTime() - DateTime.UtcNow).ToString("d\\ hh\\:mm\\:ss");
        else
            textRT.text = (spawnTime.ToUniversalTime() - DateTime.UtcNow).ToString("d\\ hh\\:mm\\:ss");
    }
}
