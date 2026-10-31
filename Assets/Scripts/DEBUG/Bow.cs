using UnityEngine;

/// <summary>
/// Luk s natahováním. Držíš tlačítko = BeginDraw(), pustíš = ReleaseArrow().
/// Síla výstřelu roste s časem natahování (0..1), příliš krátké natažení se zruší.
/// </summary>
public class Bow : MonoBehaviour
{
    [Header("Šíp")]
    public GameObject arrow;
    public Transform shootPoint;
    public Transform aimPoint;

    [Header("Natahování")]
    public float maxDrawTime = 1f;
    public float minDrawTime = 0.15f;
    public float minLaunchForce = 4f;
    public float maxLaunchForce = 20f;
    public AnimationCurve powerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public Transform nockedArrow;
    public float pullDistance = 0.4f;

    /// <summary>True, dokud hráč luk natahuje.</summary>
    public bool IsDrawing { get; private set; }

    /// <summary>Natažení 0..1. Hodí se pro UI ukazatel nebo zpomalení hráče.</summary>
    public float Charge { get; private set; }

    private float _drawTime;
    private Vector2 _aimDirection = Vector2.right;
    private Vector3 _nockedRestLocalPos;

    private void Awake()
    {
        if (nockedArrow != null)
        {
            _nockedRestLocalPos = nockedArrow.localPosition;
            nockedArrow.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Míření: luk se otáčí k aimPoint.
        Vector2 toAim = aimPoint.position - transform.position;
        if (toAim.sqrMagnitude > 0.0001f)
        {
            _aimDirection = toAim.normalized;
            float angle = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (!IsDrawing) return;

        _drawTime += Time.deltaTime;
        Charge = Mathf.Clamp01(_drawTime / maxDrawTime);

        if (nockedArrow != null)
            nockedArrow.localPosition = _nockedRestLocalPos - Vector3.right * (pullDistance * Charge);
    }

    /// <summary>Začni natahovat (stisk tlačítka).</summary>
    public void BeginDraw()
    {
        if (IsDrawing) return;

        IsDrawing = true;
        _drawTime = 0f;
        Charge = 0f;

        if (nockedArrow != null)
        {
            nockedArrow.localPosition = _nockedRestLocalPos;
            nockedArrow.gameObject.SetActive(true);
        }
    }

    /// <summary>Vystřel s aktuálním natažením (puštění tlačítka).</summary>
    public void ReleaseArrow()
    {
        if (!IsDrawing) return;

        bool drawnEnough = _drawTime >= minDrawTime;
        float power = powerCurve.Evaluate(Charge);

        CancelDraw();

        if (!drawnEnough) return;

        Debug.Log($"Spawn arrow rotation: {shootPoint.rotation.eulerAngles.z}°");
        GameObject newArrow = Instantiate(arrow, shootPoint.position, shootPoint.rotation);
        Rigidbody2D rb = newArrow.GetComponent<Rigidbody2D>();
        rb.AddForce(_aimDirection * Mathf.Lerp(minLaunchForce, maxLaunchForce, power), ForceMode2D.Impulse);
    }

    /// <summary>Zruš natahování bez výstřelu (např. při přepnutí zbraně nebo úderu).</summary>
    public void CancelDraw()
    {
        IsDrawing = false;
        _drawTime = 0f;
        Charge = 0f;

        if (nockedArrow != null)
            nockedArrow.gameObject.SetActive(false);
    }
}