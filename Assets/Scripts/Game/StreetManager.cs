using System;
using System.Collections.Generic;
using Core.System;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

public class SegmentData {
    public Segment segmentRef;
    public BoxCollider col;
    public Transform transform;
}

public class StreetManager : MonoBehaviour {
    [TitleGroup("Spawn settings")] [SerializeField]
    private List<GameObject> segmentPrefabs = new();

    [SerializeField] private int initCount;
    [SerializeField] private float triggerHeight;
    [SerializeField] private Transform spawnParent;

    [TitleGroup("Segment settings")] [SerializeField]
    private float moveSpeed;

    [SerializeField] private LayerMask playerMask;

    private List<SegmentData> _spawnedSegments = new();
    private float _sumLength;
    private BoxCollider _col;

    private BoxCollider col {
        get {
            if (!_col) _col = GetComponent<BoxCollider>();
            return _col;
        }
    }

    private readonly Random random = new();

    private void Awake() {
        for (int i = 0; i < initCount; i++) {
            CreateSegment();
        }
        UpdateCollider();
        UpdateSegments();
    }

    private void Update() {
        UpdateSegments();
    }

    private void UpdateSegments() {
        var segment = _spawnedSegments[1];
        var center = transform.worldToLocalMatrix.MultiplyPoint3x4(segment.transform.position);
        col.center = new Vector3(0f, triggerHeight * 0.5f, center.z);
    }

    public void SpawnSegments() {
        _sumLength -= _spawnedSegments[0].col.size.z;
        CreateSegment();
        Invoke(nameof(DestroyFirstSegment), 0.1f);
    }

    private void DestroyFirstSegment() {
        Destroy(_spawnedSegments[0].segmentRef.gameObject);
        _spawnedSegments.RemoveAt(0);
    }

    private void UpdateCollider() {
        var segment = _spawnedSegments[1];
        var size = segment.col.size;
        col.size = new Vector3(size.x, triggerHeight, size.z);
    }

    private void CreateSegment() {
        // Getting a random segment and spawning it.
        var obj = segmentPrefabs[random.Next(0, segmentPrefabs.Count)];
        var segment = Instantiate(obj, transform.position, Quaternion.identity, spawnParent);
        var segmentComp = segment.GetComponent<Segment>();

        // Getting collider sizes
        var segCol = segment.GetComponent<BoxCollider>();
        var bounds = segCol.bounds;
        var size = bounds.size.z;
        var offset = bounds.extents.z;

        // Offsetting the spawned segment
        segment.transform.position += new Vector3(0f, 0f, _sumLength + offset);
        _spawnedSegments.Add(new SegmentData {
            segmentRef = segmentComp,
            col        = segCol,
            transform  = segment.transform
        });

        // Increasing sum length
        _sumLength += size;

        // Setting the move speed
        segmentComp.moveSpeed = moveSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        if (LayerUtil.CompareLayer(other.gameObject.layer, playerMask)) {
            SpawnSegments();
            UpdateCollider();
        }
    }
}