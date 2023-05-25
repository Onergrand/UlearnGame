using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RoguelikeGame.Entities.Objects;

public class Button : IEntity, ISolid
{
    public int ImageId { get; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }
    public RectangleCollider Collider { get; set; }

    private bool _isClicked;
    public bool IsHovered;
    public readonly string Text;
    public readonly SpriteFont Font;
    public readonly Color TextColor;
    public readonly Color DefaultColor;
    public readonly Color HoverColor;

    public Button(int imageId, Vector2 position, int buttonWidth, int buttonHeight, string text, SpriteFont font,
        Color textColor, Color defaultColor, Color hoverColor)
    {
        ImageId = imageId;
        Position = position;
        Text = text;
        Font = font;
        TextColor = textColor;
        DefaultColor = defaultColor;
        HoverColor = hoverColor;
        
        Collider = new RectangleCollider((int)position.X, (int)position.Y, buttonWidth, buttonHeight);
    }

    public void Update() {}
    public void Update(MouseState mouseState)
    {
        var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
        if (mouseRectangle.Intersects(Collider.Boundary))
        {
            IsHovered = true;
            
            _isClicked = mouseState.LeftButton == ButtonState.Pressed;
        }
        else
        {
            IsHovered = false;
            _isClicked = false;
        }
    }
    
    public bool IsClicked() => _isClicked;

    public void MoveCollider(Vector2 newPos) { }
}