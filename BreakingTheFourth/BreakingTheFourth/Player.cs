﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//updated namespaces
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BreakingTheFourth
{
    //Contributors:
    //Kat Weis - movement, collision detection, basis (fields, constructor, properties), player lives, gravity, offset methods, original
    //draw method, more movement and collision detection
    // Matt Lienhard - helped with Offset method. All the animation - added the necessary fields, added the UpdateAnimation method
    // and the DrawWalking method and implemented it in Draw. Did the logic for flipping the player based on the mouse position and also
    // for when the player is walking backwards

    
    class Player
    {
        //Here is the code for the player
        //Will need to store a texture and a rectangle
        private Texture2D playerTexture;
        private Texture2D walkingTexture;
        private Rectangle position;
        //also needs field to detect falling
        private bool isFalling;
        private bool isJumping; //variable for determining if player jumped recently
        private int startingY; //variable for Y before jumping
        private bool canJump;
        private bool justTeleported; //variable to determine if the player just teleported
        private bool onST; //variable that is true when standing on special terrain that is moving horizontally
        private bool onVST; //variable that is true when standing on special terrain that is moving vertically
        //field for player lives
        private int playerLives;
        int screenCounter = 1;
        Level1 level1 = new Level1();
        
        public enum PlayerState
        {
            faceLeft,
            faceRight,
            walkLeft,
            walkRight,
            faceLeftWalkRight,
            faceRightWalkLeft 
        }
        private PlayerState pState;
        
        // animation fields
        private int frame;
        private double fps;
        private double timePerFrame;
        private double timeCounter;

        // constants for the source Rect
        const int WalkFrameCount = 8;
        const int Yoffset = 50;
        const int SourceHeight = 300;
        const int SourceWidth = 156;

        // FileIO object
        FileIO movement = new FileIO(1);
        //Since we don't have collectibles, we probably won't need a GameObject class
        //Make a constructor that takes 4 parameters, the x, the y, the width and the height.
        public Player(int x, int y, int width, int height)
        {
            position = new Rectangle(x, y, width, height);
            startingY = y;
            canJump = true;
            playerLives = 3;
            isJumping = false;
            isFalling = false;
            justTeleported = false;
            onST = false;
            onVST = false;
            pState = PlayerState.faceRight;

            fps = 10.0;
            timePerFrame = 1.0 / fps;
            frame = 1;
        }
        //make properties for the texture and the position & X , Y coords
        public Texture2D PlayerTexture
        {
            get { return playerTexture; }
            set { playerTexture = value; }
        }

        public Texture2D WalkingTexture
        {
            get { return walkingTexture; }
            set { walkingTexture = value; }
        }

        public Rectangle Position
        {
            get { return position; }
        }
        public int X
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public int Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        //properties for isFalling
        public bool IsFalling
        {
            get { return isFalling; }
            set { isFalling = value; }
        }
        public PlayerState PState
        {
            get { return pState; }
        }
        //property for isJumping
        public bool IsJumping
        {
            get { return isJumping; }
            set { isJumping = value; }
        }
        //property for startingY
        public int StartingY
        {
            get { return startingY; }
            set { startingY = value; }
        }
        //property for lives
        public int PlayerLives
        {
            get
            {
                if(playerLives < 0)
                {
                    playerLives = 0;
                }
                return playerLives;
            }
            set { playerLives = value; }
        }
        public void Update(KeyboardState kbState, KeyboardState previousKbState, List<Terrain> terrain, Gun gun, 
            GameState gamestate, MouseState mState, Game1 game1, Bullet bullet)
        {
            // determining movement and player orientation
            switch (pState)
            {
                case PlayerState.faceRight:
                    if(kbState.IsKeyDown(Keys.D))
                    {
                        pState = PlayerState.walkRight;                 
                    }
                    if(kbState.IsKeyDown(Keys.A))
                    {
                        pState = PlayerState.faceRightWalkLeft;
                    }
                    if(mState.X <= X + 25)
                    {
                        pState = PlayerState.faceLeft;
                    }
                    break;

                case PlayerState.faceLeft:
                    if(kbState.IsKeyDown(Keys.A))
                    {
                        pState = PlayerState.walkLeft;                       
                    }
                    if(kbState.IsKeyDown(Keys.D))
                    {
                        pState = PlayerState.faceLeftWalkRight;
                    }
                    if(mState.X > X + 25)
                    {
                        pState = PlayerState.faceRight;
                    }
                    break;

                case PlayerState.walkRight:
                    if(kbState.IsKeyDown(Keys.D))
                    {
                        if(!kbState.IsKeyDown(Keys.A))
                        {
                            X += movement.PlayerSpeed;
                        }
                    }
                    if (kbState.IsKeyUp(Keys.D))
                    {
                        pState = PlayerState.faceRight;                       
                    }
                    if(mState.X <= X + 25)
                    {
                        pState = PlayerState.faceLeftWalkRight;
                    }
                    break;

                case PlayerState.walkLeft:
                    if(kbState.IsKeyDown(Keys.A))
                    {
                        if(!kbState.IsKeyDown(Keys.D))
                        {
                            X -= movement.PlayerSpeed;
                        }
                    }
                    
                    if (kbState.IsKeyUp(Keys.A))
                    {
                        pState = PlayerState.faceLeft;
                    }
                    if (mState.X > X + 25)
                    {
                        pState = PlayerState.faceRightWalkLeft;                        
                    }
                    break;

                case PlayerState.faceLeftWalkRight:
                    if(kbState.IsKeyDown(Keys.D))
                    {
                        if(!kbState.IsKeyDown(Keys.A))
                        {
                            X += movement.PlayerSpeed;
                        }
                    }
                    if(kbState.IsKeyUp(Keys.D))
                    {
                        pState = PlayerState.faceLeft;
                    }
                    break;

                case PlayerState.faceRightWalkLeft:
                    if (kbState.IsKeyDown(Keys.A))
                    {
                        if(!kbState.IsKeyDown(Keys.D))
                        {
                            X -= movement.PlayerSpeed;
                        }
                    }
                    if (kbState.IsKeyUp(Keys.A))
                    {
                        pState = PlayerState.faceRight;
                    }
                    break;

            }
            bool collided = false;
            //collision detection
            for (int i = 0; i < terrain.Count; i++)
            {
                //doesn't allow collision if platform isn't there/has disappeared
                if (terrain[i] is DisappearingPlatforms)
                {
                    justTeleported = false;
                    DisappearingPlatforms plat = (DisappearingPlatforms)terrain[i];
                    if ((plat.Type == DisappearingPlatforms.Disappear.Blinking && plat.Tint == Color.Transparent)
                        || plat.Type == DisappearingPlatforms.Disappear.Intangible)
                    {
                        continue;
                    }
                    if (plat.CollisionDetected(position) == true)
                    {
                        canJump = false;
                        //stops no clip issues
                        Offset(terrain, kbState, i, bullet, game1);
                        //halts jumping after colliding
                        isJumping = false;
                        collided = true;
                    }
                }
                if (terrain[i].CollisionDetected(position) == true )/////special terrain is causing issue still when going down-resolved
                {
                    if(terrain[i] is DeathObject)
                    {
                        game1.Death();
                    }
                    if (terrain[i] is LevelGoal)
                    {
                        if (terrain[i].CollisionDetected(Position) == true) 
                        {
                            justTeleported = false;
                            game1.PreGamestate = gamestate;
                            game1.Gamestate = GameState.LevelClear;
                        }
                    }
                    canJump = false;
                    //stops no clip issues
                    Offset(terrain, kbState, i, bullet, game1);
                    //halts jumping after colliding
                    isJumping = false;
                    collided = true;
                }
                //fixes stuttering when moving down on a moving platform
                if(!(position.Left > terrain[i].Position.Right) && !(position.Right < terrain[i].Position.Left))
                {
                    if (terrain[i] is SpecialTerrain)
                    {
                        if (!(terrain[i] is DeathObject || terrain[i] is LevelGoal) && 
                            (Y <= terrain[i].Y - position.Height && Y >= terrain[i].Y - position.Height - movement.Gravity))//limits y range of activation
                        {
                            SpecialTerrain st = (SpecialTerrain)terrain[i];
                            if(st.MinX ==-1 && isJumping == false)
                            {
                                onVST = true;//set standing on vertical st true
                                onST = false;
                                Y = st.Position.Top - position.Height;
                                justTeleported = false;
                                isFalling = false;
                                isJumping = false;
                                startingY = position.Y;
                                collided = true;
                                canJump = true;
                            }
                            //should keep you stuck to the platform
                            else if(st.MaxY == -1)
                            {
                                if(kbState.IsKeyUp(Keys.A) && kbState.IsKeyUp(Keys.D) && kbState.IsKeyUp(Keys.Space))
                                {
                                    onST = true;
                                    onVST = false; //set standing on st true
                                    Y = st.Position.Top - position.Height;
                                    justTeleported = false;
                                    isFalling = false;
                                    isJumping = false;
                                    collided = true;
                                    canJump = true;
                                    if(st.MovingLeft == true)
                                    {
                                        X--;
                                    }
                                    else
                                    {
                                        X++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            onST = false;
                            onVST = false;
                        }
                    }
                }
                //checks if player is standing on terrain & counts that as colliding
                if(!(position.Left > terrain[i].Position.Right) && !(position.Right < terrain[i].Position.Left))
                {
                    if (position.Bottom == terrain[i].Position.Top)
                    {
                        //fixed issue where you could land on spikes
                        if (terrain[i] is DeathObject)
                        {
                            game1.Death();
                        }
                        if (terrain[i] is LevelGoal)
                        {
                            game1.PreGamestate = gamestate;
                            game1.Gamestate = GameState.LevelClear;
                        }
                        if(position.Bottom <= 0)
                        {
                            position.Y = terrain[i].Position.Top + terrain[i].Position.Height;
                        }
                        justTeleported = false;
                        isFalling = false;
                        isJumping = false;
                        startingY = position.Y;
                        collided = true;
                        canJump = true;
                    }
                }
                
            }//end of loop for collision detection
            if(isJumping == false && collided == false)
            {
                isFalling = true;
            }
            if (isFalling == true)
            {
                //go down-gravity
                Gravity();
                isJumping = false; //stops player from jumping while falling to slow descend
                canJump = false;
                justTeleported = false;//reset bool to false so it is only true for the frame exactly after teleporting
                onST = false;
                onVST = false;
            }
            if (isJumping == true)
            {
                position.Y -= 4;
                if (position.Y <= (startingY - (.75 * position.Height))) //keeps jumps to being equal to 3/4ths of the player's height
                {
                    //starts the player falling after jump is complete
                    isFalling = true;
                    onST = false;
                    onVST = false;
                }
            }
            //jump start
            else if (kbState.IsKeyDown(Keys.Space) && previousKbState.IsKeyUp(Keys.Space) && canJump == true)
            {
                //jump logic
                //go up
                position.Y -= 4;
                isJumping = true;
            }
        }//end of update method
        /// <summary>
        /// enacts falling upon the player
        /// </summary>
        public void Gravity()
        {
            position.Y += movement.Gravity;
        }
        /// <summary>
        /// Updates the frames for player animation
        /// </summary>
        /// <param name="gametime"></param>
        public void UpdateAnimation(GameTime gameTime)
        {
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeCounter >= timePerFrame)
            {
                if (PState == PlayerState.faceLeft || PState == PlayerState.faceRight || PState == PlayerState.walkLeft || PState == PlayerState.walkRight)
                {
                    frame++;
                    if (frame > WalkFrameCount)
                    {
                        frame = 1;
                    }
                }
                else
                {
                    frame--;
                    if(frame < 1)
                    {
                        frame = WalkFrameCount;
                    }
                }
                timeCounter -= timePerFrame;
            }
        }
        
        // draw the character when they're facing left
        public void DrawPlayerStanding(SpriteEffects effect, SpriteBatch sb)
        {
            sb.Draw(playerTexture, position, null, Color.White, 0, Vector2.Zero, effect, 0);
        }

        // draw the character when they're walking
        public void DrawWalking(SpriteEffects effect, SpriteBatch sb)
        {
            sb.Draw(WalkingTexture, // Texture
                Position, // Position
                new Rectangle(frame * SourceWidth, // X
                Yoffset, // Y
                SourceWidth, // Width
                SourceHeight), // Height
                Color.White, // Color
                0, // Rotation
                Vector2.Zero, // Origin
                effect, // Effects
                0); // Depth
        }
        /// <summary>
        /// draws the player to the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(playerTexture, position, Color.White);
            
            // draw the character based on their orientation
            switch (pState)
            {
                case PlayerState.faceRight:
                    DrawPlayerStanding(SpriteEffects.None, spriteBatch);
                    break;

                case PlayerState.faceLeft:
                    DrawPlayerStanding(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case PlayerState.walkRight:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;

                case PlayerState.walkLeft:
                    DrawWalking(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case PlayerState.faceLeftWalkRight:
                    DrawWalking(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case PlayerState.faceRightWalkLeft:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;
            }
        }
        //We'll probably need math calculations for velocity and stuff
        //Need method for checking collisions with walls and other objects - in terrain
        //Need to decide whether to make player move around level or level move around player

        public void Offset(List<Terrain> terrain, KeyboardState kbState, int i, Bullet bullet, Game1 game)
        {
            if(terrain[i] is DisappearingPlatforms)
            {
                terrain[i] = (DisappearingPlatforms)terrain[i];
            }
            //determines if the player is running into a wall
            if (position.Bottom > terrain[i].Position.Top + movement.Gravity && isJumping == false)
            {
                if (position.Right > terrain[i].Position.Left && kbState.IsKeyDown(Keys.D))
                {
                    position.X = terrain[i].Position.Left - position.Width;
                    //position.X -= movement.PlayerSpeed;
                }
                else if (position.Left < terrain[i].Position.Right && kbState.IsKeyDown(Keys.A) )
                {
                    position.X = terrain[i].Position.Right;
                    //position.X += movement.PlayerSpeed;
                }
                //fixes issue where when standing on moving platforms allowed you to go through walls
                else if (position.Right > terrain[i].Position.Left && position.Left < terrain[i].Position.Left  && onST == true)
                {
                    position.X = terrain[i].Position.Left - position.Width;
                    //position.X -= movement.PlayerSpeed;
                }
                else if (position.Left < terrain[i].Position.Right && position.Right > terrain[i].Position.Right && onST == true)
                {
                    position.X = terrain[i].Position.Right;
                    //position.X += movement.PlayerSpeed;
                }
                else if (onVST == true && position.Top < terrain[i].Position.Bottom)
                {
                    //kills you if you're standing on a moving platform and are squished vertically
                    game.Death();
                }
            }
            //sets player on top of terrain if fell
            if (position.Bottom <= terrain[i].Position.Top + movement.Gravity && position.Bottom > terrain[i].Position.Top)
            {
                position.Y -= position.Bottom - terrain[i].Position.Top;
                isFalling = false;
                startingY = position.Y;
                canJump = true;
            }
            if (startingY > terrain[i].Position.Bottom && IsJumping == true) // starts below the object & jumps
            {
                if (position.Top < terrain[i].Position.Bottom && IsJumping == true && position.Top > terrain[i].Position.Top)
                {
                    position.Y += terrain[i].Position.Bottom - position.Top;
                }
            }
            else if(startingY - position.Height < terrain[i].Position.Bottom && IsJumping == true) // jumps and hits an object from the side
            {
                if (position.Right > terrain[i].Position.Left && kbState.IsKeyDown(Keys.D))
                {
                    position.X = terrain[i].Position.Left - position.Width;
                    //position.X -= movement.PlayerSpeed;
                }
                else if (position.Left < terrain[i].Position.Right && kbState.IsKeyDown(Keys.A))
                {
                    position.X = terrain[i].Position.Right;
                    //position.X += movement.PlayerSpeed;
                }
            }
            if(position.Top < terrain[i].Position.Top && position.Bottom > terrain[i].Position.Top && justTeleported == true)
            {
                position.Y -= position.Bottom - terrain[i].Position.Top;
                isFalling = false;
                startingY = position.Y;
                canJump = true;
            }
            //should only activate the frame after teleporting
            else if (position.Top < terrain[i].Position.Bottom && position.Bottom > terrain[i].Position.Bottom  && justTeleported == true)
            {
                position.Y += terrain[i].Position.Bottom - position.Top;
            }
        }//end of offset method

        /// <summary>
        /// offset method specifically for teleporting
        /// </summary>
        /// <param name="terrain">list of terrain</param>
        /// <param name="i">index of terrain list the player is currently colliding with</param>
        /// <param name="bullet"></param>
        public void OffsetTele(List<Terrain> terrain, int i, Bullet bullet)//maybe do y offset first?
        {
            justTeleported = true;
            //sets player on top of terrain if teleporting downwards
            if (position.Bottom > terrain[i].Position.Top && position.Top < terrain[i].Position.Top && startingY < position.Y)
            {
                position.Y -= position.Bottom - terrain[i].Position.Top;
                isFalling = false;
                startingY = position.Y;
            }
            //if player is not in the ceiling, don't teleport them under the terrain platform
            if(position.Top > terrain[i].Position.Top + 40)
            {
                if (position.Right > terrain[i].Position.Left && bullet.FacingLeft == false)
                {
                    //offset when teleporting right
                    position.X = terrain[i].Position.Left - position.Width;
                }
                if (position.Left < terrain[i].Position.Right && bullet.FacingLeft == true)
                {
                    //offset when teleporting left
                    position.X = terrain[i].Position.Right;
                }
            }
            //deals with teleporting upwards into bottom of platform
            else if (position.Top < terrain[i].Position.Bottom && startingY > terrain[i].Position.Bottom)
            {
                position.Y += terrain[i].Position.Bottom - position.Top;
            }
            else if (position.Bottom > terrain[i].Position.Top)
            {
                if (position.Right > terrain[i].Position.Left && bullet.FacingLeft == false)
                {
                    //offset when teleporting right
                    position.X = terrain[i].Position.Left - position.Width;
                }
                if (position.Left < terrain[i].Position.Right && bullet.FacingLeft == true)
                {
                    //offset when teleporting left
                    position.X = terrain[i].Position.Right;
                }
            }
        }//end of offset tele method
    }
}
