using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

/// <summary>
/// Resolves dependencies for the entire scene.
/// </summary>
public class ResolveScene : MonoBehaviour
{
    private DependencyResolver dependencyResolver;
    /// <summary>
    /// Resolve scene dependencies on awake.
    /// </summary>
    void Awake()
    {
        dependencyResolver = new DependencyResolver();
        resolveScene();
    }

    public void resolveScene()
    {
        dependencyResolver.ResolveScene();
    }
}

