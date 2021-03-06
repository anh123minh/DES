//=============================================================================
//=  $Id: Beta.cs 185 2006-10-14 18:53:40Z eroe $
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
using System.Collections.Generic;
using System.Text;

namespace React.Distribution
{
    /// <summary>
    /// Generates random values according to a <em>beta</em>
    /// distribution.
    /// </summary>
    public class Beta : NonUniform
    {
        /// <summary>
        /// The default scale parameter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This value is used by the constructors that do not take an explicit
        /// scale value.
        /// </para>
        /// <para>
        /// The default scale is defined as
        /// <see cref="Gamma.DefaultScale">Gamma.DefaultScale</see>
        /// </para>
        /// </remarks>
        public const double DefaultScale = Gamma.DefaultScale;
        /// <summary>
        /// The default shape parameter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This value is used by the constructors that do not take an explicit
        /// shape value.
        /// </para>
        /// <para>
        /// The default shape is defined as
        /// <see cref="Gamma.DefaultShape">Gamma.DefaultShape</see>
        /// </para>
        /// </remarks>
        public const double DefaultShape = Gamma.DefaultShape;

        /// <summary>
        /// The scale parameter.  Also referred to as <em>alpha</em>.
        /// </summary>
        private double _scale;  // alpha
        /// <summary>
        /// The shape parameter.  Also referred to as <em>beta</em>.
        /// </summary>
        private double _shape;  // beta

        /// <overloads>
        /// Create and initialize a Beta random number generator.
        /// </overloads>
        /// <summary>
        /// Create a <see cref="Beta"/> random number generator.
        /// </summary>
        /// <remarks>
        /// The <see cref="Scale"/> and <see cref="Shape"/> for the
        /// <see cref="Beta"/> are set to <see cref="DefaultScale"/> and
        /// <see cref="DefaultShape"/> respectively.  The underlying
        /// <see cref="Uniform"/> generator is obtained from the
        /// default set of <see cref="Uniform"/> generators (see
        /// <see cref="UniformStreams.DefaultStreams"/>).
        /// </remarks>
        public Beta()
            : this(DefaultScale, DefaultShape)
        {
        }

        /// <summary>
        /// Create an <see cref="Beta"/> random number generator that
        /// has the given shape parameter.
        /// </summary>
        /// <remarks>
        /// The underlying <see cref="Uniform"/> generator is obtained from the
        /// default set of <see cref="Uniform"/> generators (see
        /// <see cref="UniformStreams.DefaultStreams"/>).
        /// </remarks>
        /// <param name="scale">
        /// The scale parameter.  This value is often referred to as
        /// <em>alpha</em>.</param>
        /// <param name="shape">
        /// The shape parameter.  This value is often referred to as
        /// <em>beta</em>.
        /// </param>
        public Beta(double scale, double shape)
        {
            this.Scale = scale;
            this.Shape = shape;
        }

        /// <summary>
        /// Create an <see cref="Beta"/> random number generator that
        /// obtains its underlying <see cref="Uniform"/> generator from the
        /// given <see cref="IUniformSource"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Scale"/> and <see cref="Shape"/> for the
        /// <see cref="Beta"/> are set to <see cref="DefaultScale"/> and
        /// <see cref="DefaultShape"/> respectively.
        /// </remarks>
        /// <param name="source">
        /// The <see cref="IUniformSource"/> from which this
        /// <see cref="Beta"/> can obtain its underlying
        /// <see cref="Uniform"/> generator.
        /// </param>
        public Beta(IUniformSource source)
            : this(source, DefaultScale, DefaultShape)
        {
        }

        /// <summary>
        /// Create an <see cref="Beta"/> random number generator that
        /// obtains its underlying <see cref="Uniform"/> generator from the
        /// given <see cref="IUniformSource"/> and has the given scale and
        /// shape parameters.
        /// </summary>
        /// <param name="source">
        /// The <see cref="IUniformSource"/> from which this
        /// <see cref="Beta"/> can obtain its underlying
        /// <see cref="Uniform"/> generator.
        /// </param>
        /// <param name="scale">
        /// The scale parameter.  This value is often referred to as
        /// <em>alpha</em>.</param>
        /// <param name="shape">
        /// The shape parameter.  This value is often referred to as
        /// <em>beta</em>.
        /// </param>
        public Beta(IUniformSource source, double scale, double shape)
            : base(source)
        {
            this.Scale = scale;
            this.Shape = shape;
        }

        /// <summary>
        /// Gets or sets the scale parameter.
        /// </summary>
        /// <remarks>
        /// The scale parameter is often shown as <em>alpha</em>.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// If an attempt is made to set this property to a value less than or
        /// equal to zero (0.0).
        /// </exception>
        /// <value>
        /// The scale parameter as a <see cref="double"/>.
        /// </value>
        public double Scale
        {
            get { return _scale; }
            set
            {
                if (value == 0.0)
                    throw new ArgumentException("Scale cannot be zero.");
                _scale = value;
            }
        }

        /// <summary>
        /// Gets or sets the shape parameter.
        /// </summary>
        /// <remarks>
        /// The shape parameter is often shown as <em>beta</em>.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// If an attempt is made to set this property to a value less than or
        /// equal to zero (0.0).
        /// </exception>
        /// <value>
        /// The shape parameter as a <see cref="double"/>.
        /// </value>
        public double Shape
        {
            get { return _shape; }
            set
            {
                if (value == 0.0)
                    throw new ArgumentException("Shape cannot be zero.");
                _shape = value;
            }
        }

        /// <summary>
        /// Generates the next random value according to a Beta
        /// distribution.
        /// </summary>
        /// <returns>
        /// The next random value.
        /// </returns>
        public override double NextDouble()
        {
            IUniform u = GetUniform();
            double gamma = Gamma.Generate(u, Scale, 1.0);

            return gamma == 0.0 ? 0.0 :
                gamma / (gamma + Gamma.Generate(u, Shape, 1.0));
        }
    }
}
