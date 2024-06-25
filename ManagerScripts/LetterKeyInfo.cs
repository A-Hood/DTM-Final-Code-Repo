using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script serves to hold a dictionary of hash keys assigned to each character.
/// </summary>

public class LetterKeyInfo : MonoBehaviour
{
    Dictionary<string, int> letterHashkey = new Dictionary<string, int>();

    private void Awake()
    {
        letterHashkey.Add("a", 243789);
        letterHashkey.Add("b", 897789);
        letterHashkey.Add("c", 298879);
        letterHashkey.Add("d", 768678);
        letterHashkey.Add("e", 374586);
        letterHashkey.Add("f", 678768);
        letterHashkey.Add("g", 894388);
        letterHashkey.Add("h", 345345);
        letterHashkey.Add("i", 856434);
        letterHashkey.Add("j", 198276);
        letterHashkey.Add("k", 167823);
        letterHashkey.Add("l", 896756);
        letterHashkey.Add("m", 918373);
        letterHashkey.Add("n", 238975);
        letterHashkey.Add("o", 873467);
        letterHashkey.Add("p", 674832);
        letterHashkey.Add("q", 189738);
        letterHashkey.Add("r", 453789);
        letterHashkey.Add("s", 527039);
        letterHashkey.Add("t", 253078);
        letterHashkey.Add("u", 142379);
        letterHashkey.Add("v", 352478);
        letterHashkey.Add("w", 987645);
        letterHashkey.Add("x", 197428);
        letterHashkey.Add("y", 465937);
        letterHashkey.Add("z", 798043);
        letterHashkey.Add(".", 123789);
        letterHashkey.Add("!", 876432);
        letterHashkey.Add("?", 999743);
    }

    // Return hashkey from given character.
    public int ReturnHashkey(string character)
    {
        int hashkey = 0;
        letterHashkey.TryGetValue(character, out hashkey);
        if (hashkey == 0) {
            Debug.Log("No hashkey found from given character.");
        } else {
            return hashkey;
        }
        return 0;
    }
}
