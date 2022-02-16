using System.Collections.Generic;
using UnityEngine;
using Algorithms;
using System.Linq;
using System.Collections;

public class DelayedMessageInfo<MessageCategory, Info>
    where MessageCategory : MessageEnum where Info : struct
{
    public IMessageHandler<MessageCategory, Info>[] receivers;
    public int messageLayer;
    public Info info;
    public float delayedTime;

    private DelayedMessageInfo(int pMessageLayer, Info pInfo, float pTime)
    {
        messageLayer = pMessageLayer;
        info = pInfo;
        delayedTime = pTime;
    }

    public DelayedMessageInfo(IMessageHandler<MessageCategory, Info> pReceiver, int pMessageLayer, Info pInfo, float pTime) : this(pMessageLayer, pInfo, pTime)
    {
        receivers = new IMessageHandler<MessageCategory, Info>[] { pReceiver };
    }

    public DelayedMessageInfo(List<IMessageHandler<MessageCategory, Info>> pReceivers, int pMessageLayer, Info pInfo, float pTime) : this(pMessageLayer, pInfo, pTime)
    {
        receivers = pReceivers.ToArray();
    }
}

public class MessageManager<MessageCategory, Info>
    : MonoBehaviour
    where MessageCategory : MessageEnum
    where Info : struct
{
    public float delayedTimestep = .02f;

    private List<DelayedMessageInfo<MessageCategory, Info>> delayedInfoList;

    private MinPQ<DelayedMessageInfo<MessageCategory, Info>> pq;
    private static MessageManager<MessageCategory, Info> m_manager;

    public static MessageManager<MessageCategory, Info> Instance
    {
        get
        {
            if (!m_manager)
            {
                m_manager = FindObjectOfType<MessageManager<MessageCategory, Info>>();
                if (!m_manager)
                {
                    Debug.LogError("There needs to be one active" + nameof(MessageCategory) + " Manager script on a GameObject in your scene.");
                }
                else
                {
                    m_manager.Init();
                }
            }

            return m_manager;
        }
    }

    private void Init()
    {
        //Init
        delayedInfoList = new List<DelayedMessageInfo<MessageCategory, Info>>();
        pq = new MinPQ<DelayedMessageInfo<MessageCategory, Info>>(delayedInfoList.ToArray(), DelayedTimeComparison);
    }

    private void Start()
    {
        StartCoroutine(nameof(DelayedMessageRoutine));
    }

    private static void Shout(IMessageHandler<MessageCategory, Info> receiver, int messageLayer, Info info = default)
    => receiver.OnReceivingMessage(messageLayer, info);

    public static void Shout(int messageLayer, Info info, params IMessageHandler<MessageCategory, Info>[] pReceivers)
    {
        foreach (var rc in pReceivers)
            Shout(rc, messageLayer, info);
    }

    public void DelayedShout(int messageLayer, float delayedTime, IMessageHandler<MessageCategory, Info> receiver, Info info = default)
    {
        DelayedMessageInfo<MessageCategory, Info> delayedInfo = new DelayedMessageInfo<MessageCategory, Info>(receiver, messageLayer, info, delayedTime);
        pq.Insert(delayedInfo);
    }

    public static void DelayedShout(CombatMessage msgCombatMessageype, float delayedTime, params IMessageHandler<MessageCategory, Info>[] pReceivers)
    {
        foreach (var rc in pReceivers)
            DelayedShout(msgCombatMessageype, delayedTime, rc);
    }

    private IEnumerator DelayedMessageRoutine()
    {
        while (true)
        {
            //Decrease delayed time
            if (delayedInfoList.Count > 0)
            {
                delayedInfoList.ForEach(info => info.delayedTime -= delayedTimestep);

                //Send message
                while (pq.Count > 0 && (pq.Min.delayedTime <= 0f))
                {
                    Shout(pq.Min.messageLayer, pq.Min.info, pq.Min.receivers);
                    delayedInfoList.Remove(pq.DeleteMin());
                }
            }
            yield return new WaitForSeconds(delayedTimestep);
        }
    }

    private int DelayedTimeComparison(DelayedMessageInfo<MessageCategory, Info> x, DelayedMessageInfo<MessageCategory, Info> y)
    {
        return x.delayedTime.CompareTo(y);
    }
}