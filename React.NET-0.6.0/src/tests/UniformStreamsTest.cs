//=============================================================================
//=  $Id: UniformStreamsTest.cs 140 2006-04-09 20:43:07Z Eric Roe $
//=
//=  React.NET: A discrete-event simulation library for the .NET Framework.
//=  Copyright (c) 2005, Eric K. Roe.  All rights reserved.
//=
//=  React.NET is free software; you can redistribute it and/or modify it
//=  under the terms of the GNU General Public License as published by the
//=  Free Software Foundation; either version 2 of the License, or (at your
//=  option) any later version.
//=
//=  React.NET is distributed in the hope that it will be useful, but WITHOUT
//=  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
//=  FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
//=  more details.
//=
//=  You should have received a copy of the GNU General Public License along
//=  with React.NET; if not, write to the Free Software Foundation, Inc.,
//=  51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//=============================================================================

using System;
using NUnit.Framework;
using React.Distribution;

namespace React
{
    /// <summary>
    /// Summary description for UniformStreamsTest
    /// </summary>
    [TestFixture]
    public class UniformStreamsTest
    {
        public UniformStreamsTest()
        {
        }

        [SetUp]
        public void SetUp()
        {
            // Ensure each test starts with a new default set of
            // IUniform generators.
            UniformStreams.DefaultStreams = null;
        }

        /// <summary>
        /// The default streams have the correct length.
        /// </summary>
        [Test]
        public void DefaultDefaultStream()
        {
            UniformStreams s = UniformStreams.DefaultStreams;
            Assert.AreEqual(UniformStreams.DefaultStreamCount, s.Length);
        }

        /// <summary>
        /// GetUniform is sequential and circular.
        /// </summary>
        [Test]
        public void GetUniformAndDirect()
        {
            UniformStreams s = UniformStreams.DefaultStreams;
            IUniform u1 = null, u2 = null;
            for (int i = 0; i < s.Length; i++)
            {
                u1 = s.GetUniform();
                Assert.AreEqual(s[i], u1);
                if (u2 != null)
                {
                    Assert.AreNotEqual(u1, u2);
                }
                u2 = u1;
            }

            Assert.AreEqual(s[0], s.GetUniform());
        }
    }
}
