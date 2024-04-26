# Good Apple

A simple program that can play short videos in a command prompt window. Originally created to play "[Bad Apple](https://youtu.be/FtutLA63Cp8)" but can really play anything you give it. Uses dotnet 6.0, [ffmpeg](https://ffmpeg.org/), [ffmpegcore](https://github.com/rosenbjerg/FFMpegCore), and [cscore](https://github.com/filoe/cscore). <br><b>Note, does not work in terminal, only works in the real command prompt</b>

## How to run
1) Download and extract the latest release from the [releases](https://github.com/RonaldKirk443/good-apple/releases) tab
2) Download [ffmpeg](https://ffmpeg.org/) and put the bin folder in the ffmpeg folder
3) Replace input.mp4 with the video you want to play
4) Modify settings in settings.ini (optional)
5) Run "Good Apple.exe"

## Settings
Settings name | Description
| :--- | :---
shader  |  Determines the shaders level of detail (1-4) where 1 is least and 4 is most detailed.
video-name  | Name of the video file the program looks for in the root directory.
max-realtime-fps  |  Videos with fps higher than this value will be pre-rendered, thus taking longer, to ensure smooth playback. <br> Lower this value if playback is slow or lagging.
volume  |  The volume of the video (0-100).
ffmpeg-path  |  The path to the bin folder of ffmpeg

## Video Showcase
[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/oGb05dZKznc/0.jpg)](https://www.youtube.com/watch?v=oGb05dZKznc)

## Screenshots
![Screenshot 1](https://i.imgur.com/5nYVpRT.jpg)
![Screenshot 2](https://i.imgur.com/9OVPiWi.jpg)
