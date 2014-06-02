using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vocaluxe.Base;
using VocaluxeLib.Songs;

using System.Drawing;
using Vocaluxe.Base.Fonts;
using VocaluxeLib;

class PointsPercentage
{
    private static PointsPercentage instance;

    private int playerNum;
    private CSongNote curNote;
    //private CSongLine curLine;
    private CSong song;
    private double waterLevel;
    private double pointsToShower;

    SColorF gray;
    SColorF blue;
    int y;

    private double[] currentPlayerPoints;
    //private double maxPointsCurrentLine;
    private double maxPointsCurrentNote;
    private double maxPointsSong;
    //private double currentPointsPercentage;
    private PointsPercentage()
    {
        playerNum = CConfig.NumPlayer;

        currentPlayerPoints = new double[playerNum];
        for (int i = 0; i < playerNum; i++)
        {
            currentPlayerPoints[i] = 0;
        }
        maxPointsCurrentNote = 0;
        pointsToShower = 75;
        waterLevel = 0;
        curNote = null;
        gray = new SColorF(1f, 1f, 1f, 0.5f);
        blue = new SColorF(0f, 0f, 1f, 0.7f);
        y = 100;

    }
    public static PointsPercentage Instance
    {
        get
        {
            if (instance == null)
            {

                instance = new PointsPercentage();
            }
            return instance;
        }


    }

    public void updateNote(CSongNote note)
    {
        if (curNote != note)
        {
            noteEnded();
            curNote = note;
            maxPointsCurrentNote = note.Points;
        }
    }

    private void noteEnded()
    {
        waterLevel += maxPointsCurrentNote - currentPlayerPoints[0];
        currentPlayerPoints[0] = 0;

        if (waterLevel >= pointsToShower)
        {
            showerHim();
            waterLevel = 0;
        }
    }

    public void updateTime(float time)
    {
       //
    }

    public void addPoints(int player, double points)
    {
        currentPlayerPoints[player] += points;
    }

    public void updateSong(CSong song_)
    {
        song = song_;
        maxPointsSong = song.Notes.GetVoice(0).Points;
         
    }
    private void showerHim()
    {
        //phidget stuff here!
    }

    public void Draw()
    {

        RectangleF rectWhite = new RectangleF(CSettings.RenderW - 500, y, 500, 100);
        RectangleF rectBlue = new RectangleF(CSettings.RenderW - (int)(500 * (waterLevel / pointsToShower)), y, 500, 100);

        CDraw.DrawColor(gray, new SRectF(rectWhite.X, rectWhite.Top, rectWhite.Width, rectWhite.Height, CSettings.ZNear));
        CDraw.DrawColor(blue, new SRectF(rectBlue.X, rectBlue.Top, rectBlue.Width, rectBlue.Height, CSettings.ZNear));        
    }
}

