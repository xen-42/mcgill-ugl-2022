using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A wrapper class for enum types that makes Enum support Generics and Inheritance
/// </summary>
public abstract class MessageEnum
{
    internal static int maxIndex = 0;

    public int enumValue;

    public static int GetMessageLayerMask(params MessageEnum[] msgs)
    {
        int i = 0;
        foreach (var msg in msgs)
            i |= msg.enumValue;
        return i;
    }
}

public class InteractionMessage : MessageEnum
{
    internal new static int maxIndex = MessageEnum.maxIndex;

    //Enum Types
    public static InteractionMessage Grab = new InteractionMessage();
    public static InteractionMessage Drop = new InteractionMessage();
    public static InteractionMessage Open = new InteractionMessage();
    public static InteractionMessage Dash = new InteractionMessage();
    public static InteractionMessage Close = new InteractionMessage();

    private InteractionMessage()
    {
        enumValue = 1 << (++maxIndex);
    }

    /// <summary>
    /// Additional Info for Interaction
    /// </summary>
    public struct InteractionMessageInfo
    {
    }
}

public class CombatMessage : MessageEnum
{
    internal new static int maxIndex = MessageEnum.maxIndex;

    public static CombatMessage Demage = new CombatMessage();
    public static CombatMessage Slience = new CombatMessage();
    public static CombatMessage Stun = new CombatMessage();
    public static CombatMessage Blind = new CombatMessage();
    public static CombatMessage Sleep = new CombatMessage();

    public CombatMessage()
    {
        enumValue = 1 << (++maxIndex);
    }

    /// <summary>
    /// Additional Info for Combat
    /// </summary>
    public struct CombatMessageInfo
    {
        private float demage;   //how much damage it shoud gave
        private float controlTime;  //how long should the receiver be controlled
    }
}

public interface IMessageHandler<MessageCatogary, Info> where MessageCatogary : MessageEnum where Info : struct
{
    //wtf csharp version for unity is wayyy much older than the latest
    //doesn't support default interface implementation
    //public int GetmaxIndex() { return 0; }

    void OnReceivingMessage(int messageLayermask, Info info = default);
}