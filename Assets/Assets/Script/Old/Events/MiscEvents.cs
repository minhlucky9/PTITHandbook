using System;

public class MiscEvents
{
    public event Action onCoinCollected;
    public void CoinCollected() 
    {
        if (onCoinCollected != null) 
        {
            onCoinCollected();
        }
    }

    public event Action onGemCollected;
    public void GemCollected() 
    {
        if (onGemCollected != null) 
        {
            onGemCollected();
        }
    }

    public event Action onStarCollected;
    public void StarCollected()
    {
        if (onStarCollected != null)
        {
            onStarCollected();
        }
    }
    public event Action onStarFestivalCollected;
    public void StarFestivalCollected()
    {
        if (onStarFestivalCollected != null)
        {
            onStarFestivalCollected();
        }
    }

    public event Action onMazeCollected;
    public void MazeCollected()
    {
        if (onMazeCollected != null)
        {
            onMazeCollected();
        }
    }
    public event Action onMazeRetry;
    public void MazeRetry()
    {
        if (onMazeRetry != null)
        {
            onMazeRetry();
        }
    }


    public event Action onMazeToTheTopCollected;
    public void MazeToTheTopCollected()
    {
        if (onMazeToTheTopCollected != null)
        {
            onMazeToTheTopCollected();
        }
    }
}
