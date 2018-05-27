﻿using System;
using UnityEngine;

public class HorizontalTilingBackground 
{
    const float BgChangeTolerance = 0f;

    readonly CameraWorldWrapper _cameraWorldWrapper;
    readonly SpriteInWorldMover _bgMover;
    readonly SpriteInWorldMover _copyBgMover;

    public HorizontalTilingBackground(Camera camera, SpriteRenderer sprite, SpriteRenderer spriteCopy)
    {
        if(! BackgroundsDimsEqual(sprite, spriteCopy))
            { throw new ArgumentException("backgrounds must have equal sizes"); }

        _cameraWorldWrapper = new CameraWorldWrapper(camera);

        if(! BackgroundLargerThanScreen(sprite, _cameraWorldWrapper)) 
            { throw new ArgumentException("background must be larger than screen (in world units)"); }

        _bgMover = new SpriteInWorldMover(sprite, sprite.gameObject);
        _copyBgMover = new SpriteInWorldMover(spriteCopy, spriteCopy.gameObject);

        // place the one copy of bg at the center of the camera, the other tiled to the left
        _bgMover.MoveCenter(_cameraWorldWrapper.Position);
        TileSpriteToTheLeft(_bgMover, _copyBgMover);
    }

    public void Update()
    {
        TileAuxBackgroundIfCameraOutOfBounds();
    }

    void TileAuxBackgroundIfCameraOutOfBounds()
    {
        // get currently shown background
        var currentlyShownBg = _bgMover.Contains(_cameraWorldWrapper.Position) ?
            _bgMover : _copyBgMover;

        var otherBg = GetOtherBackground(currentlyShownBg);


        //Debug.Log($"BG LB: {currentlyShownBg.LeftBound}, CAM LB: {_cameraWorldWrapper.LeftBound}," + 
        //          $" OUT? : {CameraOutOfLeftBound(currentlyShownBg)}");

        if (CameraOutOfRightBound(currentlyShownBg) && otherBg.Center.x < currentlyShownBg.Center.x)
        {
            Debug.Log("tiled to right");
            TileSpriteToTheRight(currentlyShownBg, otherBg);
        }
        else if (CameraOutOfLeftBound(currentlyShownBg) && otherBg.Center.x > currentlyShownBg.Center.x)
        {
            Debug.Log("tiled to left");
            TileSpriteToTheLeft(currentlyShownBg, otherBg);
        }
    }

    /// <summary>
    /// tiles one sprite to the right of another:
    /// places the <paramref name="movingSprite"/> so that it's top left corner 
    /// is at the <paramref name="staticSprite"/>'s top right corner, 
    /// making a seemless connection
    /// </summary>
    static void TileSpriteToTheRight(SpriteInWorldMover staticSprite, SpriteInWorldMover movingSprite)
    {
        movingSprite.MoveTopLeftCorner(staticSprite.TopRight);
    }

    /// <summary>
    /// tiles one sprite to the left of another:
    /// places the <paramref name="movingSprite"/> so that it's top right corner 
    /// is at the <paramref name="staticSprite"/>'s top left corner, 
    /// making a seemless connection
    /// </summary>
    static void TileSpriteToTheLeft(SpriteInWorldMover staticSprite, SpriteInWorldMover movingSprite)
    {
        movingSprite.MoveTopRightCorner(staticSprite.TopLeft);
    }

    bool CameraOutOfRightBound(SpriteInWorldMover sprite)
    {
        return _cameraWorldWrapper.RightBound + BgChangeTolerance >= sprite.RightBound;
    }

    bool CameraOutOfLeftBound(SpriteInWorldMover sprite)
    {
        return _cameraWorldWrapper.LeftBound - BgChangeTolerance <= sprite.LeftBound;
    }

    SpriteInWorldMover GetOtherBackground(SpriteInWorldMover thisSprite)
    {
        return thisSprite == _bgMover ?
            _copyBgMover : _bgMover;
    }

    #region validation
    static bool BackgroundsDimsEqual(SpriteRenderer bg, SpriteRenderer otherBg)
    {
        return bg.bounds.size == otherBg.bounds.size;
    }

    static bool BackgroundLargerThanScreen(SpriteRenderer bg, CameraWorldWrapper camera)
    {
        var cameraArea =  camera.GetScreenDimsInWorldCoords().magnitude;
        var backgroundDims = bg.bounds.size;
        backgroundDims.z = 0;

        return backgroundDims.magnitude > cameraArea;
    }
    #endregion
}
