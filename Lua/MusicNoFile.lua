local dfpwm = require("cc.audio.dfpwm")
local speakers = { peripheral.find("speaker") }
local volume = 50
local decoder = dfpwm.make_decoder()
local song = "Song"
local fileDownload, reason = http.get("http://162.231.184.144:8080/Music/convert/" .. arg[1], nil, true)
local data = fileDownload.readAll()
while(true)
do
for i = 1, 1, 1
do
  for i = 1, #data, 16*64 do
    local buffer = decoder(data:sub(i,i+16*64-1))

      while not speakers[1].playAudio(buffer, volume) do
          os.pullEvent("speaker_audio_empty")
      end
    end
end

end
