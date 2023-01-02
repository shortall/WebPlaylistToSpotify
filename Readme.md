![App Screenshot](AppScreenshot.png)

# To use the app

## Step 1 - Configure

The configuration file is appsettings.json

|Setting|Description|
|-------|-----------|
|SpotifyApiToken|Create youself a token here https://developer.spotify.com/|
|SpotifyUsername|Spotify username, regular or email|
|WebPlaylists|A collection of web playlists containing details of source playlists|

### WebPlaylist

```json
{
  "Url": "https://www.bbc.co.uk/programmes/articles/5JDPyPdDGs3yCLdtPhGgWM7/bbc-radio-6-music-playlist",
  "TrackNamesXPath": "(//div[@class='text--prose']/p)[position()>1]"
}
```
|Property|Description|
|-------|-----------|
|Url|URL of a HTML or XML based playlist|
|TrackNamesXPath|XPath expression to select nodes containing the track names. Inner text will be extracted from each node.|

## Step 2 - Run

Run WebPlaylistToSpotify.exe

