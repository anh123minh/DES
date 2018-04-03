<?xml version="1.0" encoding="iso-8859-1"?>
<!--
 *
 * $Id: convert.xslt 137 2006-01-22 19:42:03Z Eric Roe $
 *
 * XSLT stylesheet used to convert XML files located in the 'xml' directory
 * into HTML files for the React.NET web site.
 *
 *                             - * - * - * -
 *
 * Copyright © 2005 Eric K. Roe.  All rights reserved.
 *
 * React.NET is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * React.NET is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with React.NET; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 *
 -->

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" version="1.0" encoding="iso-8859-1" indent="yes"/>
	
	<xsl:variable name="title" select="web-page/@title"/>
	<xsl:variable name="ver">0.5</xsl:variable>
	<xsl:variable name="sfimglink"><!--
		-->http://sflogo.sourceforge.net/sflogo.php?group_id=152956&amp;type=1<!--
	--></xsl:variable>
	
	<xsl:param name="csspath">../css</xsl:param>
	<xsl:param name="imgpath">../images</xsl:param>
	<xsl:param name="sflogo">false</xsl:param>
	
	
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	
<!-- ********************************************************************** -->
	<xsl:template match="web-page">
		<xsl:text disable-output-escaping="yes">
<![CDATA[
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN"
    "http://www.w3.org/TR/html4/strict.dtd">
 ]]>
        </xsl:text>
<html>
	<head>
		<title>React.NET - <xsl:value-of select="string($title)"/></title>
		<meta http-equiv="Content-Language" content="en-us"/>
		<meta name="copyright" content="Copyright © 2006 Eric K. Roe, All rights reserved."/>
		<link rel="stylesheet" type="text/css" href="{$csspath}/style.css"/>
	</head>
	<body>
		<!-- ============================================================== -->
		<div id="banner">
			<a name="top"></a><!-- Needed for Firefox. -->
			<h1 id="title">React.NET</h1>
			<h2 id="slogan">Discrete Event Simulation Framework</h2>
		</div>
		<!-- ============================================================== -->
		<div id="menubar">
			<p>
				|<a href="index.html"> Home </a>
				|<a href="quick_start.html">Quick Start</a>
				|<a href="examples.html"> Examples </a>
				|<a href="howtos.html"> How-tos </a>
				|<a href="documentation.html"> Documentation </a>
				<!-- |<a href="development.html"> Development </a> -->
				|
			</p>
		</div>
		<!-- ============================================================== -->
		<xsl:if test="@preliminary = 'true'">
			<p class="preliminary">
				[This is preliminary documentation and is subject to change.]
			</p>
		</xsl:if>
		<!-- ============================================================== -->
		<div id="content">
			<xsl:if test="not(boolean(@skip-toc))">
				<xsl:call-template name="make-toc" />
			</xsl:if>
			<div>
				<xsl:comment>===================================================</xsl:comment>
				<xsl:apply-templates/>
				<xsl:comment>===================================================</xsl:comment>
			</div>
		</div>
		<!-- ============================================================== -->
		<div id="footer">
			<hr/>
			<img id="valid" src="http://www.w3.org/Icons/valid-html401"
				alt="Valid HTML 4.01 Strict" height="31" width="88"/>
			<xsl:if test="boolean(@skip-toc)">
				<a href="http://sourceforge.net/">
					<xsl:if test="$sflogo = 'true'">
						<img style="margin-left: 5px; float: left;"
							src="{$sfimglink}"
							width="88" height="31" alt="SourceForge.net Home" />
					</xsl:if>
				</a>
			</xsl:if>
			<address>
				<xsl:text disable-output-escaping="yes">&amp;copy;</xsl:text>
				2006 Eric K. Roe.  All rights reserved.
			</address>
		</div>
	</body>
</html>
	</xsl:template>
<!-- ********************************************************************** -->

	<xsl:template name="make-toc">
		<div style="float: right; background-color: white; padding: 15px; border: none;">
			<div id="toc">
				<p>Contents</p>
				<ul>
					<xsl:call-template name="make-toc-entry" />
				</ul>
				<div style="text-align: center; margin: 0px auto;">
					<a href="http://sourceforge.net" title="SourceForge.net Home">
						<xsl:if test="$sflogo = 'true'">
							<img style="margin: 8px;"
								src="http://sflogo.sourceforge.net/sflogo.php?group_id=152956&amp;type=1"
								width="88" height="31" alt="SourceForge.net Home" />
						</xsl:if>
					</a>
				</div>
			</div>
		</div>
	</xsl:template>
	
	<xsl:template name="make-toc-entry">
		<xsl:for-each select="//h1">
			<li><div>
				<a>
					<xsl:attribute name="href">#<xsl:value-of select="a/@name"/></xsl:attribute>
					<xsl:value-of select="text()"/>
				</a>
			</div></li>
		</xsl:for-each>
		<xsl:call-template name="howto-toc-entry" />
	</xsl:template>
	
	<xsl:template name="howto-toc-entry">
		<xsl:for-each select="//how-to-section">
			<li>
				<a>
					<xsl:attribute name="href">#<xsl:value-of select="@id"/></xsl:attribute>
					<xsl:value-of select="@name"/>
				</a>
				
			</li>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="react">
		<xsl:text>React.NET</xsl:text>
	</xsl:template>
		
	<xsl:template match="block">
		<xsl:if test="boolean(@div)">
			<div>
				<xsl:if test="@class">
					<xsl:attribute name="class"><xsl:value-of select="@class"/></xsl:attribute>
				</xsl:if>
				<xsl:apply-templates />
			</div>
		</xsl:if>
		<xsl:if test="not(@div)">
			<xsl:apply-templates />
		</xsl:if>
	</xsl:template>

<!-- ********************************************************************** -->
	<xsl:template match="note">
		<xsl:call-template name="callout">
			<xsl:with-param name="label">Note</xsl:with-param>
			<xsl:with-param name="color">#4A86B8</xsl:with-param>
			<xsl:with-param name="background">#B9D1E4</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template match="warning">
		<xsl:call-template name="callout">
			<xsl:with-param name="label">Important</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template match="annotation">
		<xsl:param name="label" select="@label" />
		<xsl:param name="fgcolor" select="@color" />
		<xsl:param name="bgcolor" select="@background" />
		<xsl:call-template name="callout">
			<xsl:with-param name="label"><xsl:value-of select="$label"/></xsl:with-param>
			<xsl:with-param name="color"><xsl:value-of select="$fgcolor"/></xsl:with-param>
			<xsl:with-param name="background"><xsl:value-of select="$bgcolor"/></xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template name="callout">
		<xsl:param name="label">Note</xsl:param>
		<xsl:param name="color">red</xsl:param>
		<xsl:param name="background">pink</xsl:param>
		<div style="margin: 0px auto; text-align: center;">
			<div class="callout">
				<xsl:attribute name="style">
					border-color: <xsl:value-of select="$color"/>;
					background-color: <xsl:value-of select="$background"/>;
				</xsl:attribute>
				<div class="annotation">
					<xsl:attribute name="style">
						background-color: <xsl:value-of select="$color"/>
					</xsl:attribute>
					<xsl:value-of select="$label"/>
				</div>
				<div class="message">
					<xsl:apply-templates/>
				</div>
			</div>
		</div>
	</xsl:template>
	
<!-- ********************************************************************** -->
	<xsl:template match="example">
		<div class="example"><pre style="margin: 0px;">
			<xsl:value-of select="text()"/></pre>
		</div>
	</xsl:template>
	
	<xsl:template match="classname">
		<span class="classname"><xsl:value-of select="text()"/></span>
	</xsl:template>
	
<!-- ********************************************************************** -->
	<xsl:template match="how-to-index">
		<xsl:for-each select="following-sibling::how-to-section">
			<h2><xsl:value-of select="@name"/></h2>
			<ul class="howto">
				<xsl:for-each select="how-to">
					<li>
						<a>
							<xsl:attribute name="href">
								<xsl:value-of select="concat('#', ../@id, string(position()))" />
							</xsl:attribute>
							<xsl:value-of select="@title"/>
						</a>
					</li>
				</xsl:for-each>
			</ul>
		</xsl:for-each>
	</xsl:template>
	
	<xsl:template match="how-to-section">
		<h1>
			<a>
				<xsl:attribute name="name"><xsl:value-of select="@id"/></xsl:attribute>
				<xsl:value-of select="@name"/>
			</a>
		</h1>
		<xsl:apply-templates select="description/node()" />
		<div>
			<xsl:apply-templates select="how-to"/>
		</div>
	</xsl:template>

	<xsl:template match="how-to">
		<h2>
			<a href="#top" title="Back to top">
				<img class="totop" src="{$imgpath}/to_top.gif" alt="Back to top"/>
			</a>
			<a>
				<xsl:attribute name="name">
					<xsl:value-of select="concat(../@id, string(position()))" />
				</xsl:attribute>
				<xsl:value-of select="string(position())"/>. <xsl:value-of select="@title" />
			</a>
		</h2>
		<xsl:apply-templates />
	</xsl:template>
	
</xsl:stylesheet>
