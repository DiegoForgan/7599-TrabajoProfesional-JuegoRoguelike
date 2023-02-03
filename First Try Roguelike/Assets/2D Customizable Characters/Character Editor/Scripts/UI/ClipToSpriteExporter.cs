using System.IO;
using UnityEngine;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class ClipToSpriteExporter
    {
        ~ClipToSpriteExporter()
        {
            if (_renderTexture != null)
                _renderTexture.Release();
        }

        private Vector2Int _frameResolution;
        private RenderTexture _renderTexture;
        private Color _clearColor;
        private int _rows;
        private int _columns;
        private Texture2D _texture;
        private int _createdSheets;
        private string _filePath;
        private bool _hasFramesToSave;

        public void Prepare(Vector2Int frameResolution, Color clearColor, int rows, int columns, string savePath)
        {
            _frameResolution = frameResolution;
            _clearColor = clearColor;
            _rows = rows;
            _columns = columns;
            _createdSheets = 0;
            _filePath = savePath;
            _hasFramesToSave = false;

            if (_renderTexture != null)
                _renderTexture.Release();
            _renderTexture = CreateRenderTexture(_frameResolution.x, _frameResolution.y, _clearColor);

            var textureResolution = _frameResolution;
            textureResolution.x *= _columns;
            textureResolution.y *= _rows;
            _texture = CreateTexture(textureResolution);
        }

        public void CaptureFrame(Bounds bounds, Camera camera)
        {
            if (_renderTexture == null)
            {
                Debug.LogError("No RenderTexture has been created. Make sure to run the Prepare method first.");
                return;
            }

            ClearRenderTexture(_renderTexture, _clearColor);
            var resolution = _frameResolution;
            var orthographicSize = bounds.extents.y;

            // cache camera values
            var originalClearFlags = camera.clearFlags;
            var originalBackgroundColor = camera.backgroundColor;
            var originalViewportRect = camera.rect;
            var originalPosition = camera.transform.position;
            var originalSize = camera.orthographicSize;

            // set values for capture
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = _clearColor;
            camera.rect = new Rect(0, 0, resolution.x, resolution.y);
            camera.transform.position = bounds.center;
            camera.orthographicSize = orthographicSize;
            camera.targetTexture = _renderTexture;
            camera.Render();

            // reset camera
            camera.clearFlags = originalClearFlags;
            camera.backgroundColor = originalBackgroundColor;
            camera.rect = originalViewportRect;
            camera.transform.position = originalPosition;
            camera.orthographicSize = originalSize;
            camera.targetTexture = null;
        }

        public void TrySaveAnimationFrame(int frameCount)
        {
            var framesPerImage = _columns * _rows;
            var amountOfFramesOnImages = _createdSheets * framesPerImage;
            AddAnimationFrameToTexture(frameCount - amountOfFramesOnImages);
            _hasFramesToSave = true;

            var wasLastFrameOnImage = frameCount - amountOfFramesOnImages >= framesPerImage;
            if (wasLastFrameOnImage)
            {
                SaveTexture(_texture, true);
                ClearTexture(_texture);
                _createdSheets++;
                _hasFramesToSave = false;
            }
        }

        public void Complete()
        {
            if (_hasFramesToSave)
                SaveTexture(_texture, true);

            _renderTexture.Release();
            _renderTexture = null;
            _texture = null;
        }

        public string GetFullSavePath()
        {
            var fullPath = _filePath;
            var isSheet = _rows > 1 || _columns > 1;
            if (isSheet)
                fullPath += " Sheet";

            fullPath += (_createdSheets + 1).ToString("D3") + ".png";
            return fullPath;
        }

        private RenderTexture CreateRenderTexture(int width, int height, Color clearColor)
        {
            // var renderTexture = RenderTexture.GetTemporary(width, height, 32,
            //     RenderTextureFormat.ARGB32,
            //     RenderTextureReadWrite.Default);
            var renderTexture =
                new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            renderTexture.useMipMap = false;
            renderTexture.filterMode = FilterMode.Point;
            renderTexture.anisoLevel = 0;
            renderTexture.antiAliasing = 1;
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            ClearRenderTexture(renderTexture, clearColor);
            return renderTexture;
        }

        private void ClearRenderTexture(RenderTexture renderTexture, Color clearColor)
        {
            if (renderTexture == null)
                return;

            RenderTexture.active = renderTexture;
            var backgroundColor = clearColor;
            GL.Clear(true, true, backgroundColor, 1.0f);
            RenderTexture.active = null;
        }

        private Texture2D CreateTexture(Vector2Int resolution)
        {
            var texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false, false);

            ClearTexture(texture);
            return texture;
        }

        private void ClearTexture(Texture2D texture)
        {
            Color32 resetColor = new Color32(255, 255, 255, 0);
            Color32[] resetColorArray = texture.GetPixels32();

            for (int i = 0; i < resetColorArray.Length; i++)
            {
                resetColorArray[i] = resetColor;
            }

            texture.SetPixels32(resetColorArray);
            texture.Apply();
        }

        private void AddAnimationFrameToTexture(int frame)
        {
            RenderTexture.active = _renderTexture;
            var row = (int)Mathf.Ceil((float)frame / _columns);
            var frameOnRow = frame - ((row - 1) * _columns);
            var x = (frameOnRow - 1) * _frameResolution.x;
            var y = row * _frameResolution.y;
            y = _texture.height - y; // invert for top left coordinates
            _texture.ReadPixels(new Rect(0, 0, _frameResolution.x, _frameResolution.y), x, y);
            _texture.Apply();
            RenderTexture.active = null;
            FixTransparencyBackgroundColor(_texture, x, y, _frameResolution.x, _frameResolution.y);
        }

        private void FixTransparencyBackgroundColor(Texture2D texture, int fromX, int fromY, int width, int height)
        {
            // An attempt at removing background color from semi-transparent pixels.
            // Ideally should be done by removing premultiplied alpha and dilating the colors.

            var pixels = texture.GetPixels(fromX, fromY, width, height);

            for (int i = 0; i < pixels.Length; i++)
            {
                var pixel = pixels[i];
                var alpha = pixel.a;
                var missingColor = pixel * (1 - alpha);
                var newColor = pixel + missingColor;
                newColor.a = alpha;
                pixels[i] = newColor;
            }

            texture.SetPixels(fromX, fromY, width, height, pixels);
            texture.Apply();
        }

        private void SaveTexture(Texture2D texture, bool withLogging)
        {
            var fullPath = GetFullSavePath();
            File.WriteAllBytes(fullPath, texture.EncodeToPNG());

            if (withLogging)
                Debug.Log("Sprite was exported to " + fullPath);
        }
    }
}