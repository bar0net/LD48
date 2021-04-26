using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootShoot : MonoBehaviour
{
    public bool active = true;

    public GameObject point;
    public GameObject segment;
    public float growTime = 0.25f;
    public float ratioIncrease = 0.2f;
    public float growRange = 2.0f;

    struct TransformData
    {
        public TransformData(float rotation, float scale) {this.rotation = rotation; this.scale = scale; }
        public override string ToString() {return rotation.ToString() + " | " + scale.ToString(); }
        public float rotation;
        public float scale;
    }
    
    List<Vector3> landmarks;
    List<Transform> segments;

    List<TransformData> origins, targets;
    List<float> timers;
    bool readyToSplit = false;

    HashSet<Tile> magnets;
    //
    // Needs
    //

    public Vector2 goal;
    Plant _plant;
    SoilManager _soil;

    // Start is called before the first frame update
    void Start()
    {
        landmarks = new List<Vector3>();
        segments  = new List<Transform>();

        origins = new List<TransformData>();
        targets = new List<TransformData>();
        timers  = new List<float>();

        _plant = FindObjectOfType<Plant>();
        _soil  = FindObjectOfType<SoilManager>();

        point.GetComponentInChildren<RootPoint>().shoot = this;

        magnets = new HashSet<Tile>();
        
        landmarks.Add(point.transform.position - 10 * Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {

        if (!active) return;
        bool done = true;
        for (int i = 0; i < timers.Count; ++i )
        {
            if (timers[i] < 0) continue;
            done = false;

            float ratio = 1 - timers[i] / (growTime + 0.000001f);
            segments[i].rotation   = Quaternion.Euler(0, 0, Mathf.Lerp(origins[i].rotation, targets[i].rotation, ratio));

            SpriteRenderer sr = segments[i].gameObject.GetComponent<SpriteRenderer>();
            if (sr != null) sr.size = new Vector2(sr.size.x, Mathf.Lerp(origins[i].scale, targets[i].scale, ratio));

            if (i == timers.Count - 1 && landmarks.Count > 1)
            {
                point.transform.position = Vector3.Lerp(landmarks[i], landmarks[i + 1], ratio) - 5.0f*Vector3.forward;
            }

            timers[i] -= Time.deltaTime;
        }

        if (done && readyToSplit) Split();
    }

    void Grow(Vector3 target)
    {
        if (timers.Count > 0 && timers[timers.Count - 1] > 0) return;

        GameObject go = (GameObject)Instantiate(segment, this.transform);

        int N = landmarks.Count - 1;
        float z = landmarks[N].z;

        landmarks.Add(new Vector3(target.x, target.y, z));
        segments.Add(go.transform);

        TransformData data = SegmentTransform(segments.Count - 1);
        origins.Add(new TransformData(data.rotation, 0));
        targets.Add(data);
        timers.Add(growTime);

        go.transform.position = new Vector3(landmarks[N].x, landmarks[N].y, z - 1);
        go.transform.rotation = Quaternion.Euler(0, 0, data.rotation);
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null) sr.size = new Vector2(sr.size.x, 0);
        else Debug.Log("No sprite renderer found.");

        point.transform.rotation = go.transform.rotation;
    }


    TransformData SegmentTransform(int index)
    {
        Vector2 start = landmarks[index];
        Vector2 end   = landmarks[index + 1]; 
        
        Vector2 diff   = end - start;
        float angle    = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        return new TransformData(90 + angle, diff.magnitude + ratioIncrease);
    }

    public float SetNextObjective()
    {
        if (magnets.Count <= 0) return RandomObjective();

        float repel = RandomObjective();

        Vector3 avg = Vector3.zero;
        foreach (Tile t in magnets) avg += t.transform.position + 0.5f * Vector3.up;
        avg /= magnets.Count;

        float y_diff = Mathf.Abs(avg.y - transform.position.y);
        float rng = Random.Range(-0.25f, 0.25f);


        float x_pos = Mathf.Clamp(rng + avg.x - repel, transform.position.x - growRange, transform.position.x + growRange);

        Debug.DrawLine(this.transform.position, this.transform.position - repel * Vector3.right, Color.cyan, 1000);

        Vector3 pos = landmarks[landmarks.Count - 1];
        Grow(new Vector3(x_pos, _plant.depth, pos.z));
        return pos.y - 1;
    }

    float RepelForce()
    {
        float repel = 0;
        foreach (RootShoot s in _plant.activeRootShoots)
        {
            if (s == this) continue;
            float dst = s.transform.position.x - transform.position.x;
            repel += Mathf.Sign(dst) / (dst * dst);
        }
        return repel;
    }

    public float RandomObjective()
    {
        float left_bound  = 1.0f - (_soil.terrainWidth / 2.0f);
        float right_bound = (_soil.terrainWidth / 2.0f) - 0.5f;

        foreach (RootShoot s in _plant.activeRootShoots)
        {
            if (s == this) continue;

            float my_x = this.transform.position.x;
            float other_x = s.transform.position.x;

            if (     other_x - 0.5f > my_x  &&  other_x - 0.5f < right_bound)  right_bound = other_x - 0.5f;
            else if (other_x + 0.5f < my_x  &&  other_x + 0.5f > left_bound )  left_bound = other_x + 0.5f;
        }

        if (Mathf.Abs(left_bound  - transform.position.x) > growRange) left_bound  = transform.position.x - growRange;
        if (Mathf.Abs(right_bound - transform.position.x) > growRange) right_bound = transform.position.x + growRange;

        Vector3 pos = landmarks[landmarks.Count - 1];
        float x_pos = Random.Range(left_bound, right_bound);

        Grow(new Vector3(x_pos, _plant.depth, pos.z));
        return pos.y - 1;
    }

    public void AddContact(Tile tile)
    {
        //contacts.Add(tile);
        _plant.AddNutrient(tile.resourceType);
    }

    public void AddMagnet(Tile tile)
    {
        magnets.Add(tile);
        tile.GetComponent<ContactCounter>().AddContact();

        SpriteRenderer[] renderers = tile.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in renderers) s.enabled = true;
    }

    public void RemoveMagnet(Tile tile)
    {
        magnets.Remove(tile);

        tile.GetComponent<ContactCounter>().RemoveContact();
        if (tile.GetComponent<ContactCounter>().HasLiveContacts()) return;

        SpriteRenderer[] renderers = tile.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in renderers) s.enabled = (s.gameObject == tile.gameObject);
    }

    public void Reproduce()
    {
        readyToSplit = true;
    }

    private void Split()
    {
        _plant.activeRootShoots.Remove(this);

        {
            Vector3 target = point.transform.position; 
            Vector3 dir = new Vector3(+0.4f, -0.3f, 0);
            GameObject go = (GameObject)Instantiate(_plant.rootShootPrefab, target+dir, Quaternion.identity, _plant.transform);
            GameObject branch = Instantiate(segment, target, Quaternion.identity, this.transform);
            float angle = 90 + Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            branch.transform.rotation = Quaternion.Euler(0, 0, angle);
            SpriteRenderer sr = branch.GetComponent<SpriteRenderer>();
            if (sr != null) sr.size = new Vector2(sr.size.x, dir.magnitude + ratioIncrease);
            _plant.AddRootShot(go.GetComponent<RootShoot>());
        }

        {
            Vector3 target = point.transform.position;
            Vector3 dir = new Vector3(-0.4f, -0.3f, 0);;
            GameObject go = (GameObject)Instantiate(_plant.rootShootPrefab, target + dir, Quaternion.identity, _plant.transform);
            GameObject branch = Instantiate(segment, target, Quaternion.identity, this.transform);
            float angle = 90 + Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            branch.transform.rotation = Quaternion.Euler(0, 0, angle);
            SpriteRenderer sr = branch.GetComponent<SpriteRenderer>();
            if (sr != null) sr.size = new Vector2(sr.size.x, dir.magnitude + ratioIncrease);
            _plant.AddRootShot(go.GetComponent<RootShoot>());
        }

        this.enabled = false;
    }

    public void Death()
    {
        _plant.activeRootShoots.Remove(this);
        if (_plant.activeRootShoots.Count == 0) FindObjectOfType<Manager>().GameOver();
    }
}
