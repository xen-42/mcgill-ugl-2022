using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class MinPQ<K> : IEnumerable<K>
    {
        private K[] _pq;
        private int _n;

        private Comparison<K> _comp;

        public int Count => _n;

        public K Min => _pq[1];

        public static MinPQ<K> operator +(MinPQ<K> pq, K k)
        {
            pq.Insert(k);
            return pq;
        }

        public MinPQ(int cap, Comparison<K> pComparer)
        {
            _pq = new K[cap + 1];
            _n = 0;
            _comp = pComparer;
        }

        public MinPQ(K[] keys, Comparison<K> pComparer)
        {
            _n = keys.Length;
            _pq = new K[keys.Length + 1];
            keys.CopyTo(_pq, 1);

            //Swim-Based Sort
            for (int i = 1; i <= _n; i++)
                Swim(i);

            _comp = pComparer;
        }

        public void Insert(K key)
        {
            if ((_n + 1) >= (_pq.Length / 2))
                Resize(2 * _pq.Length);
            _pq[++_n] = key;
            Swim(_n);
        }

        public K DeleteMin()
        {
            K max = _pq[1];
            exch(1, _n--);
            Sink(1);

            _pq[_n + 1] = default;
            if (_n < (_pq.Length / 4))
                Resize(_pq.Length / 2);

            return max;
        }

        #region Helpers

        private void Resize(int cap)
        {
            K[] tmp = new K[cap];
            _pq.CopyTo(tmp, 0);

            _pq = tmp;
        }

        private void Sink(int k)
        {
            while (2 * k <= _n)
            {
                //Find max of children

                int minIndex = (2 * k + 1 > _n || _comp(_pq[2 * k], (_pq[2 * k + 1])) < 0) ? (2 * k) : 2 * k + 1;
                if (_comp(_pq[minIndex], (_pq[k])) < 0)
                {
                    exch(k, minIndex);
                    k = minIndex;
                }
                else
                    break;
            }
        }

        private void Swim(int k)
        {
            while (k > 1 && _comp(_pq[k / 2], (_pq[k])) > 0)
            {
                exch(k, k / 2);
                k /= 2;
            }
        }

        private void exch(int a, int b)
        {
            K tmp = _pq[a];
            _pq[a] = _pq[b];
            _pq[b] = tmp;
        }

        #endregion Helpers

        #region Enumerable

        public IEnumerator<K> GetEnumerator()
        {
            return ((IEnumerable<K>)_pq).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pq.GetEnumerator();
        }

        #endregion Enumerable
    }
}