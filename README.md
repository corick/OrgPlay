OrgPlay
====

Organya (Cave Story music format) player using C# and FNA. WIP.

## Usage

        OrgPlay.exe (path to org file)

To Do:
 * Support for looping.
 * Fix drums sounding a bit funky.
 * Ensure 'PI' notes are the correct length.
 * Clean up the code (Especially the ORG loader).
 * Visualization (Show the piano roll.)
 * Windows support?

Organya File Format
----

### Org Header:
[0x00]  char[6] Magic number: Always "Org-02", presumably a version number.
[0x06]  short   Millisecond delay between beats. "Wait" param in OrgMaker.
[0x08]  byte    Time signature. The "numerator" of the time signature. Does not affect playback.
[0x09]  byte    Time Signature. The "denominator" of the time signature. Does not affect playback.
[0x0A]  int32   Loop start. (Beats/Measure) * Measure. The beat that the loop starts on.
[0x0E]  int32   Loop end. The beat that the song loops before. (Hard cap at 4080.)

### Instrument table: 
Repeat the following sixteen times.
[0x12] Start of an instrument block. 

[+0x00] short   The fine tuning parameter. 100-1900. "Voice" param in OrgMaker.
[+0x02] byte    Instrument index. 0-99. For instruments, this * 256 is the index in the wave lookup table. 
[+0x03] byte    The 'Pi' (Pizzacatto) option in OrgMaker. If set, disables sustain on the instrument. Only affects instruments, not drums. This caps the instrument to a predetermined number of samples.
[+0x04] ushort  Note Count. The amount of notes which this instrument channel uses.

### Note table: 
Each instrument has a note table, placed sequentially one after the other.
These are laid out in blocks by property. All of the starting beats for each note in a given instrument are grouped, 
then their note pitch values, then their lengths, et cetera.
[0x72] Start address of note table. Repeat this once for each instrument.

int     Beat Number- Beat which note event starts on. 
byte    Note- Represented as notes above c0. 0xFF signifies the last note's value should be used instead. (No change.)
byte    length  How long this note is held in beats. 0xFF = No change
byte    Volume- 0-254 Volume is not linear with 0 still being about 20%. 0xFF = No change
byte    Pan- From 0 - 12, with 6 being the center. 0xFF = No Change.

### wave100.dat
Each wavetable sample is 256 bytes long and consists of signed bytes. THe instrument number is used to determine the starting index in the /.

### Drum samples.
Not entirely sure how to rip these. These are big-endian signed shorts.

Playing Organya Files 
----

TO DO: Write this. 


