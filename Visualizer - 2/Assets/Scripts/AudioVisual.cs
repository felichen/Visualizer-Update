using UnityEngine;


//MAPPINGS
//SIZE OF OBJECTS IN BACKGROUND CAN GROW/SHRINK BASED ON: audiopeer's average amplitude, or certain frequency bins
//COLOR CHANGE BASED ON: different frequency bin spikes find colorchange)
public class AudioVisual : MonoBehaviour
{
    public AudioAnalyzer _audioAnalyzer;
    public GameObject CameraControl;
    public Phyllotaxis _phyllotaxis;
    public GameObject phyllotaxisball;
    public Material matRef;
    public Material particleMat;
    ParticleSystem particles;
    ParticleSystemRenderer psRenderer;
    public ParticleSystem emphasisEmitter;
    float particleSize = 0.25f;
    private const int SAMPLE_SIZE = 1024;
    float maxScale = 10.0f;
    float visualModifier = 175.0f;
    float smoothing = 20.0f; //buffer for smoother animation
    public float keep = 0.1f;
    float rotationSpeed = 10f;
    float particleThreshold = 0.5f; //threshold for particles to emit out of ends of bars

    public Color colone;
    public Color coltwo;


    //COLOR PALETTES
    //purple
    Color[] purple;
    Color[] blue;
    Color[] green;
    Color[] red;
    Color[] pink;


    //BEAT DETECTION
    public float _startScale, _scaleMultiplier;
    public int bpm;
    public float currSongTime;
    public bool isOnBeat;
    public bool spike;


    private AudioSource source;
    public float[] spectrum;

    //CIRCLE VISUALIZATION
    GameObject circleParent;
    private Transform[] cubeTransform; //contains transforms of cubes
    private Transform[] emphasisTransform;
    private float[] scaleFactor;
    private float cubeWidth = 0.3f;
    private int numVisObjects = 64; //amount of objects

    private Transform[] centerEmitterTrans;

    private Transform[] rmsTransform; //transform for rms
    private Transform[] dbTransform;
    private Transform[] pitchTransform;
    private Transform[] beatTransform;

    private GameObject[] colorCubes;

    //FLYING OBJECTS
    private Transform cameraTransform; //store position of camera
    public Transform[] flyingObjects;
    public Vector3[] finalPos;
    private float flyingSpeed = 50.0f;
    private int numFlying = 50; //number of flying objects
    private float c = 30; //variance of final pos
    private int farBack = 200; //how far back objects spawn

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -75);
        GameObject ps = GameObject.Find("Particle System");
        if (ps != null)
        {
            ps.transform.position = new Vector3(0, 0, -30);
            particles = ps.GetComponent<ParticleSystem>();
            psRenderer = ps.GetComponent<ParticleSystemRenderer>();
        }
        //CREATE PARENT
        circleParent = this.transform.GetChild(0).gameObject;

        Debug.Log("r!!: " + colone.r + "g: " + colone.g + " b: " + colone.b);

        cameraTransform = GameObject.Find("Main Camera").transform;

        source = GetComponent<AudioSource>();
        //spectrum = new float[SAMPLE_SIZE];


        //bpm = BPMAnalyzer.AnalyzeBpm(source.clip);
        //Debug.Log(string.Format("what is bpm: {0}", bpm));


        //set color palettes
        purple = new Color[2];
        blue = new Color[2];
        green = new Color[2];
        red = new Color[2];
        pink = new Color[2];

        purple[0] = new Color(78f/255f, 4f/255f, 144f/255f, 1f); //dark purple
        purple[1] = new Color(9f/255f, 142f/255f, 199f/255f,1f); //dark green blue

        blue[0] = new Color(19f / 255f, 24f / 255f, 178f / 255f, 1f); //dark blue
        blue[1] = new Color(19f / 255f, 178f / 255f, 172f / 255f, 1f); //green

        green[0] = new Color(16f / 255f, 174f / 255f, 144 / 168f, 1f); //mid bluegreen
        green[1] = new Color(131 / 255f, 212f / 255f, 125 / 255f, 1f); //light green

        red[0] = new Color(202f / 255f, 48f / 255f, 94f / 255f, 1f); //dark purple
        red[1] = new Color(173f / 255f, 88f / 255f, 209f / 255f, 1f); //dark green blue

        pink[0] = new Color(1f, 73f / 255f, 250 / 255f, 1f); //dark purple
        pink[1] = new Color(73 / 255f, 1f, 250 / 255f, 1f); //dark green blue

        //_audioAnalyzer.songBPMs.TryGetValue(source.clip.name, out bpm);

        //if (bpm <= 100)
        //{
        //    colone = purple[0];
        //    coltwo = purple[1];
        //} else if (bpm <= 150)
        //{
        //    colone = blue[0];
        //    coltwo = blue[1];
        //}
        //else if (bpm <= 190)
        //{
        //    colone = green[0];
        //    coltwo = green[1];
        //}
        //else if (bpm <= 225)
        //{
        //    colone = red[0];
        //    coltwo = red[1];
        //}
        //else 
        //{
        //    colone = pink[0];
        //    coltwo = pink[1];
        //}

        colone = pink[0];
        coltwo = pink[1];
        InstantiateCircle(); //creates circle at center of screen
        InstantiateRMSDBCube();
        //InstantiateFlying();
    }

    void InstantiateCircle()
    {
        scaleFactor = new float[numVisObjects];
        cubeTransform = new Transform[numVisObjects];
        emphasisTransform = new Transform[numVisObjects];
        centerEmitterTrans = new Transform[1];

        Vector3 center = Vector3.zero;
        float radius = 10.0f;

        //create a center particle emitter
        var centergo = new GameObject("Center Emitter");
        centergo.transform.parent = CameraControl.transform;
        centergo.AddComponent<ParticleSystem>();
        ParticleSystem pcenter = centergo.GetComponent<ParticleSystem>();

        //set material
        centergo.GetComponent<ParticleSystemRenderer>().material = particleMat;

        //change shape
        var spshape = pcenter.shape;
        ParticleSystemShapeType sphere = ParticleSystemShapeType.Sphere;
        spshape.shapeType = sphere;
        var spmain = pcenter.main;
        spmain.maxParticles = 100;
        spmain.startSpeed = 30;
        spmain.startSize = particleSize * 5;
        spmain.startColor = colone;
        pcenter.enableEmission = false;
        centerEmitterTrans[0] = pcenter.transform;

        for (int i = 0; i < numVisObjects; i++)
        {
            float ang = i * 1.0f / numVisObjects;
            ang = ang * Mathf.PI * 2;

            float x = center.x + Mathf.Cos(ang) * radius;
            float y = center.y + Mathf.Sin(ang) * radius;

            Vector3 pos = center + new Vector3(x, y, 0);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.transform.parent = circleParent.transform;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(cubeWidth, 1, 1);
            //Color col = new Color(1f - (0.01f * i), 0f + (2*Mathf.Sin(2*0.01f* i)), 0.01f*i, 1);
            Color newcol = lerp((float)i / numVisObjects);
            go.GetComponent<Renderer>().material.color = newcol;
            cubeTransform[i] = go.transform;

            ////create emitter that comes out of bars in center visual
            //create emitter that comes out of bars in center visual
            var pgo = new GameObject("PS" + i);
            pgo.AddComponent<ParticleSystem>();
            ParticleSystem p = pgo.GetComponent<ParticleSystem>();

            //set material
            pgo.GetComponent<ParticleSystemRenderer>().material = particleMat;

            //change shape
            var shape = p.shape;
            ParticleSystemShapeType mesh = ParticleSystemShapeType.Mesh;
            shape.shapeType = mesh;
            var main = p.main;
            main.maxParticles = 50;
            main.startSize = particleSize;
            main.startColor = newcol;
            //shape.angle = 0;
            p.transform.parent = circleParent.transform;
            p.transform.localPosition = go.transform.localPosition;
            p.transform.localRotation = Quaternion.LookRotation(Vector3.forward, pos);
            p.transform.Rotate(-90, 0, 0);
            p.enableEmission = false;
            //p.transform.localRotation = go.transform.localRotation;
            emphasisTransform[i] = p.transform;
        }
    }

    public Color lerp(float value)
    {
        //return new Color(colone.r * value + coltwo.r * (1 - value),
        //            colone.g * value + coltwo.g * (1 - value),
        //            colone.b * value + coltwo.b * (1 - value));
        return Color.Lerp(colone, coltwo, value);
    }

    void InstantiateRMSDBCube()
    {
        rmsTransform = new Transform[1];
        dbTransform = new Transform[1];
        pitchTransform = new Transform[1];
        beatTransform = new Transform[1];
        colorCubes = new GameObject[1];

        //GameObject rmsgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        //rmsgo.transform.position = new Vector3(-4, 0, 0);
        //rmsTransform[0] = rmsgo.transform;

        //GameObject dbgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        //dbgo.transform.position = new Vector3(-2, 0, 0);
        //dbTransform[0] = dbgo.transform;

        //GameObject pitchgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        //pitchgo.transform.position = new Vector3(0, 0, 0);
        //pitchTransform[0] = pitchgo.transform;

        GameObject beatgo = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;
        beatgo.GetComponent<Renderer>().receiveShadows = false;
        beatgo.GetComponent<Renderer>().material = matRef;
        beatgo.transform.localScale = new Vector3(2, 2, 2);
        beatTransform[0] = beatgo.transform;

        colorCubes[0] = beatgo;
    }

    void InstantiateFlying() //MUST CALL THIS EVERY SET NUMBER OF MINUTES
    {
        flyingObjects = new Transform[numFlying];
        finalPos = new Vector3[numFlying];

        for (int i = 0; i < numFlying; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;
            go.transform.localScale *= 3;
            go.transform.position = new Vector3(0, 0, farBack);
            flyingObjects[i] = go.transform;

            //choose final position
            Vector3 cam = cameraTransform.position;
            float x = Random.Range(cam.x - c, cam.x + c);
            float y = Random.Range(cam.y - c, cam.y + c);
            Vector3 end = new Vector3(x, y, cam.z - 10);
            finalPos[i] = end;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //AnalyzeSound();
        UpdateVisual();
        //UpdateRMS();
        //UpdateFlying();
        //UpdateBeat();

        float scale = _audioAnalyzer._AmplitudeBuffer * _scaleMultiplier + _startScale;
        colorCubes[0].transform.localScale = new Vector3(scale, scale, scale);

        //Debug.Log("original r: " + colone.r + "g: " + colone.g + " b: " + colone.b);
        ////update color saturation
        //float H, S, V;
        //Color.RGBToHSV(colone, out H, out S, out V);
        //Debug.Log("hsv H: " + H + "S: " + S + " V: " + V);
        //colone = Color.HSVToRGB(H, S, Mathf.Clamp(_audioAnalyzer._AmplitudeBuffer, 0f, 1f));

        //Debug.Log("new r: " + colone.r + "g: " + colone.g + " b: " + colone.b);

        //setColors();

        //Color.RGBToHSV(new Color(coltwo.r, coltwo.g, coltwo.b), out H, out S, out V);
        //coltwo = Color.HSVToRGB(H, _audioAnalyzer._AmplitudeBuffer, V);
    }

    void UpdateVisual() //modify scale of objects
    {
        //ROTATE CIRCLE PARENT
        circleParent.transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
        int index = 0;
        int spectrumIndex = 0;
        //only keep certain percentage so not every bar is flat/boring
        int averageSize = (int)((SAMPLE_SIZE * keep) / numVisObjects);


        while (index < numVisObjects)
        {
            float sum = 0;
            for (int j = 0; j < averageSize; j++)
            {
                sum += _audioAnalyzer._samplesLeft[spectrumIndex];
                spectrumIndex++;
            }

            //when moving down, slow; going up, snap
            float scaleY = sum / averageSize * visualModifier;
            scaleFactor[index] -= Time.deltaTime * smoothing; //previous value

            //if going down, scale down smoothly
            if (scaleFactor[index] < scaleY)
                scaleFactor[index] = scaleY;
            if (scaleFactor[index] > particleThreshold * maxScale)
            {
                if (circleParent.activeSelf == true)
                {
                    emitParticles(index);
                }

            }
            //if at max size, snap up
            if (scaleFactor[index] > maxScale)
            {
                //ONLY DETECT CERTAIN BINS OF FREQUENCY TO SEE IF THEY REACH MAXSCALE
                if (index >= 5 && index <= 8)
                {
                    changeColor(); //****************************************************************************************************************************************
                    if (phyllotaxisball.activeSelf == true && (index >= 6 && index <= 7))
                    {
                        emitCenterParticles();
                    }
                }
                scaleFactor[index] = maxScale;
            }

            //Vector3 newTrans = new Vector3(0.5f, 1, 1);
            cubeTransform[index].localScale = new Vector3(cubeWidth, 1, 1) + Vector3.up * scaleFactor[index];
            index++;

        }
    }

    public void setColors()
    {
        for (int i = 0; i < numVisObjects; i++) {
            Color newcol = lerp((float)i / numVisObjects);
            cubeTransform[i].GetComponent<Renderer>().material.color = newcol;
            ParticleSystem ps = emphasisTransform[i].gameObject.GetComponent<ParticleSystem>();
            var p = ps.main;
            p.startColor = newcol;
        }

    }

    void emitParticles(int i)
    {
        ParticleSystem ps = emphasisTransform[i].gameObject.GetComponent<ParticleSystem>();
        //ps.enableEmission = true;
        ps.Emit(1);
        //ps.enableEmission = false;
    }

    void emitCenterParticles()
    {
        ParticleSystem ps = centerEmitterTrans[0].gameObject.GetComponent<ParticleSystem>();
        //ps.enableEmission = true;
        ps.Emit(1);
        //ps.enableEmission = false;
    }

    void changeColor()
    {
        //psRenderer.material.color = UnityEngine.Random.ColorHSV();
        float val = Random.Range(0.0f, 1.0f);
        colorCubes[0].GetComponent<Renderer>().material.color = lerp(val);
        _phyllotaxis._trailcolor = lerp(val);

    }

    void UpdateRMS()
    {
        rmsTransform[0].localScale = Vector3.one + Vector3.up * _audioAnalyzer.RMS * 100;
        dbTransform[0].localScale = Vector3.one + Vector3.up * Mathf.Abs(_audioAnalyzer.DB) * 3;
        //check for negative infinity
        if (double.IsNegativeInfinity(_audioAnalyzer.DB))
        {
            dbTransform[0].localScale = Vector3.one + Vector3.up * 0;
        }

        pitchTransform[0].localScale = Vector3.one + Vector3.up * _audioAnalyzer.PITCH / 100;
    }

    void UpdateFlying()
    {
        float c = 100;
        for (int i = 0; i < numFlying; i++)
        {
            flyingObjects[i].localPosition = Vector3.MoveTowards(flyingObjects[i].position, finalPos[i], flyingSpeed * Time.deltaTime);
        }
    }



    bool onBeat()
    {
        currSongTime = source.time;
        float divisor = bpm / 60;
        float epsilon = 0.05f;
        if (currSongTime % divisor < epsilon)
        {
            return true;
        }
        return false;

    }

}
