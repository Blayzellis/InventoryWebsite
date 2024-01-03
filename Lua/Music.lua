local dfpwm = require("cc.audio.dfpwm")
local speakers = { peripheral.find("speaker") }
local volume = 50
local decoder = dfpwm.make_decoder()
local song = "Song"
while(true)
do
for i = 1, 1, 1
do
    for chunk in io.lines(song .. i .. ".dfpwm", 16 * 1024) do
      local buffer = decoder(chunk)

      while not speakers[1].playAudio(buffer, volume) do
          os.pullEvent("speaker_audio_empty")
      end
    end
end

end
