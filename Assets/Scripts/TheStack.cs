using UnityEngine;

public class TheStack : MonoBehaviour
{
    private UIManager uiManager;

    private const float BoundSize = 3.5f;
    private const float MovingBoundsSize = 3f;
    private const float StackMovingSpeed = 5.0f;
    private const float BlockMovingSpeed = 3.5f;
    private const float ErrorMargin = 0.1f;

    public GameObject originBlock = null;

    private Vector3 prevBlockPosition;
    private Vector3 desiredPosition;
    private Vector3 stackBounds = new Vector2(BoundSize, BoundSize);

    private Transform lastBlock = null;
    private float blockTransition = 0f;
    private float secondaryPosition = 0f;

    private bool isGameOver = true;

    private int stackCount = -1;
    public int Score { get { return stackCount; } }

    private int comboCount = 0;
    public int Combo { get { return comboCount; } }

    private int maxCombo = 0;
    public int MaxCombo { get =>  maxCombo; }

    public Color prevColor;
    public Color nextColor;

    private bool isMovingX = true;

    private int bestScore = 0;
    public int BestScore { get => bestScore; }

    private int bestCombo = 0;
    public int BestCombo { get => bestCombo; }

    private const string BestScoreKey = "BestScore";
    private const string BestComboKey = "BestCombo";

    private void Start()
    {
        uiManager = UIManager.Instance;

        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestCombo = PlayerPrefs.GetInt(BestComboKey, 0);

        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        prevBlockPosition = Vector3.down;
    }

    private void Update()
    {
        if (isGameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceBlock())
            {
                Spawn_Block();
            }
            else
            {
                Debug.Log("GameOver");
                UpdateScore();
                isGameOver = true;
                GameOverEffect();
                uiManager.SetScoreUI();
            }
        }

        MoveBlock();
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed * Time.deltaTime);
    }

    public void Restart()
    {
        int childCount = transform.childCount;

        for(int i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        isGameOver = false;

        lastBlock = null;
        desiredPosition = Vector3.zero;
        stackBounds = new Vector3(BoundSize, BoundSize);

        stackCount = -1;
        isMovingX = true;
        blockTransition = 0f;
        secondaryPosition = 0f;

        comboCount = 0;
        maxCombo = 0;

        prevBlockPosition = Vector3.down;

        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        Spawn_Block();
        Spawn_Block();
    }

    private bool Spawn_Block()
    {
        if (lastBlock != null)
            prevBlockPosition = lastBlock.localPosition;

        GameObject newBlock = null;
        Transform newTrans = null;

        newBlock = Instantiate(originBlock);

        if(newBlock == null)
        {
            Debug.Log("NewBlock Instantiate Failed!");
            return false;
        }

        newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        newTrans.localRotation = Quaternion.identity;
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stackCount++;

        desiredPosition = Vector3.down * stackCount;
        blockTransition = 0f;

        lastBlock = newTrans;

        isMovingX = !isMovingX;

        uiManager.UpdateScore();
        return true;
    }

    private void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed;

        float movePosition = Mathf.PingPong(blockTransition, BoundSize) - BoundSize * 0.5f;

        if (isMovingX)
        {
            lastBlock.localPosition = new Vector3(movePosition * MovingBoundsSize, stackCount, secondaryPosition);
        }
        else
        {
            lastBlock.localPosition = new Vector3(secondaryPosition, stackCount, -movePosition * MovingBoundsSize);
        }
    }

    private bool PlaceBlock()
    {
        Vector3 lastPosition = lastBlock.localPosition;

        if (isMovingX)
        {
            float deltaX = prevBlockPosition.x - lastPosition.x;
            bool isNegativeNum = (deltaX < 0) ? true : false;

            deltaX = Mathf.Abs(deltaX);
            if (deltaX > ErrorMargin)
            {
                stackBounds.x -= deltaX;
                if (stackBounds.x <= 0)
                {
                    return false;
                }

                float middle = (prevBlockPosition.x + lastPosition.x) * 0.5f;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPositoin = lastBlock.localPosition;
                tempPositoin.x = middle;
                lastBlock.localPosition = lastPosition = tempPositoin;

                float rubbleHalfScale = deltaX * 0.5f;
                CreateRubble(
                    new Vector3(isNegativeNum
                        ? lastPosition.x + stackBounds.x * 0.5f + rubbleHalfScale
                        : lastPosition.x - stackBounds.x * 0.5f + rubbleHalfScale
                    , lastPosition.y
                    , lastPosition.z),
                    new Vector3(deltaX, 1, stackBounds.y)
                );

                comboCount = 0;
            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }
        else
        {
            float deltaZ = prevBlockPosition.z - lastPosition.z;
            bool isNegativeNum = (deltaZ < 0) ? true : false;

            deltaZ = Mathf.Abs(deltaZ);
            if (deltaZ > ErrorMargin)
            {
                stackBounds.y -= deltaZ;
                if(stackBounds.y <= 0)
                {
                    return false;
                }

                float middle = (prevBlockPosition.z + lastPosition.z) * 0.5f;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.z = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                float rubbleHalfScale = deltaZ * 0.5f;
                CreateRubble(
                    new Vector3(isNegativeNum
                        ? lastPosition.x + stackBounds.x * 0.5f + rubbleHalfScale
                        : lastPosition.x - stackBounds.x * 0.5f + rubbleHalfScale
                    , lastPosition.y
                    , lastPosition.z),
                    new Vector3(deltaZ, 1, stackBounds.y)
                );

                comboCount = 0;
            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }

        secondaryPosition = (isMovingX) ? lastBlock.localPosition.x : lastBlock.localPosition.z;

        return true;
    }

    private void ComboCheck()
    {
        comboCount++;

        if(comboCount > maxCombo)
            maxCombo = comboCount;

        if((comboCount % 5) == 0)
        {
            Debug.Log("5Combo Success");
            stackBounds += new Vector3(0.5f, 0.5f);
            stackBounds.x = (stackBounds.x > BoundSize) ? BoundSize : stackBounds.x;
            stackBounds.y = (stackBounds.y > BoundSize) ? BoundSize : stackBounds.y;
        }
    }

    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject gameObject = Instantiate(lastBlock.gameObject);
        gameObject.transform.parent = this.transform;

        gameObject.transform.localPosition = pos;
        gameObject.transform.localScale = scale;
        gameObject.transform.localRotation = Quaternion.identity;

        gameObject.AddComponent<Rigidbody>();
        gameObject.name = "Rubble";
    }

    private void UpdateScore()
    {
        if(bestScore < stackCount)
        {
            Debug.Log("최고 점수 갱신");
            bestScore = stackCount;
            bestCombo = maxCombo;

            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.SetInt(BestComboKey, bestCombo);
        }
    }

    private void GameOverEffect()
    {
        int childCount = this.transform.childCount;

        for(int i = 1; i < 20; i++)
        {
            if (childCount < i)
                break;

            GameObject gameObject = this.transform.GetChild(childCount - i).gameObject;

            if (gameObject.name.Equals("Rubble")) continue;

            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();

            rigidbody.AddForce((Vector3.up * Random.Range(0, 10f) + Vector3.right * (Random.Range(0, 10f) - 5f)) * 100f);
        }
    }

    private Color GetRandomColor()
    {
        float r = Random.Range(100f, 250f) / 255f;
        float g = Random.Range(100f, 250f) / 255f;
        float b = Random.Range(100f, 250f) / 255f;

        return new Color(r, g, b);
    }

    private void ColorChange(GameObject gameObject)
    {
        Color applyColor = Color.Lerp(prevColor, nextColor, (stackCount % 11) / 10f);

        Renderer renderer = gameObject.GetComponent<Renderer>();

        if(renderer != null)
        {
            Debug.Log("Renderer is NULL!");
            return;
        }

        renderer.material.color = applyColor;
        Camera.main.backgroundColor = applyColor - new Color(0.1f, 0.1f, 0.1f);

        if (applyColor.Equals(nextColor))
        {
            prevColor = nextColor;
            nextColor = GetRandomColor();
        }
    }
}
