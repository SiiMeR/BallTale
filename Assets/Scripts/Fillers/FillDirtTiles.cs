using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FillDirtTiles : MonoBehaviour
{
    public Tilemap FGTilemap;

    public Tilemap MainTilemap;

    public Tile Tile;

    // down, left, up, right, sw, nw, ne, se, edgesw, edgenw, edgene, edgese,  all, ldr, lur, tld, trd, lr, ud  -- tiles in this order
    //    0,    1,  2,     3,  4,  5,  6,  7,      8,      9,     10,     11,  12 ,  13,  14,  15,  16, 17, 18
    public Tile[] Tiles;


#if UNITY_EDITOR

    /// <summary>
    ///     Fill all nearby tiles with crust tiles depending if they are empty
    /// </summary>
    public void FillAllWithDirt()
    {
        var allPositions = MainTilemap.GetTileOfTypeInAllDirections<Tile>(Tile);

        foreach (var (position, nearbySum) in allPositions.Select(x => (x.Key, x.Value)))
        {
            var tile = ScriptableObject.CreateInstance<Tile>();

            switch (nearbySum)
            {
                case 0: // no empty
                    continue;

                case 1: // down only
                case 17:
                case 33:
                case 49:
                case 65:
                case 81:
                case 97:
                case 113:
                case 129:
                case 145:
                case 161:
                case 177:
                case 193:
                case 209:
                case 225:
                case 241:
                    tile = Tiles[0];
                    break;

                case 2: // left only
                case 18:
                case 34:
                case 50:
                case 66:
                case 82:
                case 98:
                case 114:
                case 130:
                case 146:
                case 162:
                case 178:
                case 194:
                case 210:
                case 226:
                case 242:
                    tile = Tiles[1];
                    break;


                case 4: // up only
                case 20:
                case 36:
                case 52:
                case 68:
                case 84:
                case 100:
                case 116:
                case 132:
                case 148:
                case 164:
                case 180:
                case 196:
                case 212:
                case 228:
                case 244:
                    tile = Tiles[2];
                    break;


                case 8: // right only
                case 24:
                case 40:
                case 56:
                case 72:
                case 88:
                case 104:
                case 120:
                case 136:
                case 152:
                case 168:
                case 184:
                case 200:
                case 216:
                case 232:
                case 248:
                    tile = Tiles[3];
                    break;


                case 3: // d + l
                case 19:
                case 35:
                case 51:
                case 67:
                case 83:
                case 99:
                case 115:
                case 131:
                case 147:
                case 163:
                case 179:
                case 195:
                case 211:
                case 227:
                case 243:
                    tile = Tiles[4];
                    break;

                case 6: // u + l
                case 22:
                case 38:
                case 54:
                case 70:
                case 86:
                case 102:
                case 118:
                case 134:
                case 150:
                case 166:
                case 182:
                case 198:
                case 214:
                case 230:
                case 246:
                    tile = Tiles[5];
                    break;

                case 12: // r + u
                case 28:
                case 44:
                case 60:
                case 76:
                case 92:
                case 108:
                case 124:
                case 140:
                case 156:
                case 172:
                case 188:
                case 204:
                case 220:
                case 236:
                case 252:
                    tile = Tiles[6];
                    break;

                case 9: // r + d         
                case 25:
                case 41:
                case 57:
                case 73:
                case 89:
                case 105:
                case 121:
                case 137:
                case 153:
                case 169:
                case 185:
                case 201:
                case 217:
                case 233:
                case 249:
                    tile = Tiles[7];
                    break;

                case 16: // sw
                    tile = Tiles[8];
                    break;

                case 32: // nw
                    tile = Tiles[9];
                    break;

                case 64: // ne
                case 96:
                    tile = Tiles[10];
                    break;

                case 128: // se
                    tile = Tiles[11];
                    break;


                case 15: // r + u + d + l
                case 31:
                case 47:
                case 63:
                case 79:
                case 95:
                case 111:
                case 127:
                case 143:
                case 159:
                case 175:
                case 191:
                case 207:
                case 223:
                case 239:
                case 255:
                    tile = Tiles[12];
                    break;


                // ldr
                case 11: // r + d + l
                case 27:
                case 43:
                case 59:
                case 75:
                case 91:
                case 107:
                case 123:
                case 139:
                case 155:
                case 171:
                case 187:
                case 203:
                case 219:
                case 235:
                case 251:
                    tile = Tiles[13];
                    break;

                // lur
                case 14: // r + u + l
                case 30:
                case 46:
                case 62:
                case 78:
                case 94:
                case 110:
                case 126:
                case 142:
                case 158:
                case 174:
                case 190:
                case 206:
                case 222:
                case 238:
                case 254:
                    tile = Tiles[14];
                    break;

                // tld
                case 7: // u + d + l  
                case 23:
                case 39:
                case 55:
                case 71:
                case 87:
                case 103:
                case 119:
                case 135:
                case 151:
                case 167:
                case 183:
                case 199:
                case 215:
                case 231:
                case 247:
                    tile = Tiles[15];
                    break;

                // trd
                case 13: // r + u + d
                case 29:
                case 45:
                case 61:
                case 77:
                case 93:
                case 109:
                case 125:
                case 141:
                case 157:
                case 173:
                case 189:
                case 205:
                case 221:
                case 237:
                case 253:
                    tile = Tiles[16];
                    break;

                case 10:
                case 26:
                case 42:
                case 58:
                case 74:
                case 90:
                case 106:
                case 122:
                case 138:
                case 154:
                case 170:
                case 186:
                case 202:
                case 218:
                case 234:
                case 250:
                    tile = Tiles[17]; // multiples of 16
                    break;

                case 5: // u + d
                case 21:
                case 37:
                case 53:
                case 69:
                case 85:
                case 101:
                case 117:
                case 133:
                case 149:
                case 165:
                case 181:
                case 197:
                case 213:
                case 229:
                case 245:

                    tile = Tiles[18];
                    break;
            }

            FGTilemap.SetTile(position, tile);
        }
    }


    public void FillWithDirt(Vector3Int direction)
    {
        var tileType = direction == Vector3Int.down ? Tiles[0] :
            direction == Vector3Int.left ? Tiles[1] :
            direction == Vector3Int.up ? Tiles[2] :
            Tiles[3];


        var positionsInDirection = MainTilemap.GetEmptyAdjacentTilesInDirection<Tile>(direction);

        foreach (var (position, nearbySum) in positionsInDirection.Select(x => (x.Key, x.Value)))
        {
            var tilePos = position + direction;

            var tile = MainTilemap.GetTile(position);

            if (tile && tile.name.Equals(Tile.name)) FGTilemap.SetTile(tilePos, tileType);
        }
    }

    public void RemoveAllDirt()
    {
        foreach (var tile in Tiles) FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(tile);
    }

#endif
}