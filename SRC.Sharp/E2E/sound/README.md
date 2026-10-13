# sound

`StartBGM`（MIDI）と `PlaySound`（WAV）を鳴らす。音声ファイルはこのケースのために作ったもの（ド・ミ・ソの MIDI、440Hz の WAV）。

SRC#Web の E2E テストでは、効果音が Web Audio で再生されることを確かめる。MIDI は SoundFont を同梱しないため、SoundFont が無い場合に鳴らさずに進むことを確かめる。
