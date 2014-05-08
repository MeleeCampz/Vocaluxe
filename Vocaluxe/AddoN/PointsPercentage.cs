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
    private CSongLine curLine;
    private CSong song;
    private double minPercentage = 0.5;

    private string fmt;
    private string fmt2;

    private double minTime = 5.0;

    private int curPage;
    private int pages;

    private string txt;
    SColorF gray;
    int y;

    private double[] currentPlayerPoints;
    private double maxPointsCurrentLine;
    private double currentPointsPercentage;
    private PointsPercentage()
    {
        playerNum = CConfig.NumPlayer;

        currentPlayerPoints = new double[playerNum];
        for (int i = 0; i < playerNum; i++)
        {
            currentPlayerPoints[i] = 0;
        }
        maxPointsCurrentLine = 0;
        curLine = null;
        txt = "";
        gray = new SColorF(1f, 1f, 1f, 0.5f);
        y = 100;
        currentPointsPercentage = 0;
        fmt = "000.##";
        fmt2 = "00.##";
        //formatString = " {0,15:" + fmt + "}";
        //formatString2 = " {0,15:" + fmt2 + "}";

        curPage = 0;
        pages = 0;

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

    public void addPoints(int player, double points)
    {
        currentPlayerPoints[player] += points;

        currentPointsPercentage = currentPlayerPoints[player] / maxPointsCurrentLine;
        // Draw();
    }

    public void updateLine(CSongLine line)
    {
        if (curLine != line)
        {
            curPage++;
            curLine = line;
            if (curPage > pages)
            {
                showerHim();
                for (int i = 0; i < playerNum; i++)
                {
                    currentPlayerPoints[i] = 0;
                }
                maxPointsCurrentLine = 0;
                pages = 0;
                curPage = 1;
                float time = 0.0f;
                bool lastLine = false;
                CSongLine cline = line;
                int lineNumb = song.Notes.GetVoice(0).FindPreviousLine(cline.StartBeat);
                while (lastLine == false && time < minTime)
                {
                    cline = song.Notes.GetVoice(0).Lines[lineNumb];
                    pages++;
                    time += CGame.GetTimeFromBeats(cline.EndBeat, song.BPM) - CGame.GetTimeFromBeats(cline.StartBeat, song.BPM);
                    maxPointsCurrentLine += song.Notes.GetVoice(0).Lines[lineNumb].Points;
                    lineNumb++;
                    if( lineNumb >= song.Notes.GetVoice(0).Lines.Length -1)
                    {
                        lastLine = true;
                    }
                }
                CGame.GetTimeFromBeats(line.StartBeat, song.BPM);
            }
        }
    }

    public void updateSong(CSong song_)
    {
        song = song_;
    }
    private void showerHim()
    {
        for (int i = 0; i < playerNum; i++)
        {
            if (currentPointsPercentage < minPercentage)
            {
                //txt = "Player " + i.ToString() + " failed!";
                //Console.WriteLine("Player " + i.ToString() + " failed!");
            }
        }
    }

    public void Draw()
    {
        txt = "Points: " + (currentPointsPercentage * 100).ToString(fmt) + "%";
        txt += "   Page " + curPage.ToString(fmt2) + "/"+pages.ToString(fmt2);

        RectangleF rect = new RectangleF(CSettings.RenderW - CFonts.GetTextWidth(txt), y, CFonts.GetTextWidth(txt), CFonts.GetTextHeight(txt));
        CDraw.DrawColor(gray, new SRectF(rect.X, rect.Top, rect.Width, rect.Height, CSettings.ZNear));
        CFonts.DrawText(txt, rect.X, rect.Y, CSettings.ZNear);
    }
}

