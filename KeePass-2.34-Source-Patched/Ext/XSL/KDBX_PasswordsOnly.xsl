<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="text" omit-xml-declaration="yes" indent="no" encoding="UTF-8" />

<xsl:template match="/">
<xsl:apply-templates select="KeePassFile" />
</xsl:template>

<xsl:template match="KeePassFile">
<xsl:apply-templates select="Root" />
</xsl:template>

<xsl:template match="Root">
<xsl:for-each select="Group">
<xsl:apply-templates select="." />
</xsl:for-each>
</xsl:template>

<xsl:template match="Group">
<xsl:for-each select="Entry">
<xsl:apply-templates select="." />
</xsl:for-each>
<xsl:for-each select="Group">
<xsl:apply-templates select="." />
</xsl:for-each>
</xsl:template>

<xsl:template match="Entry">
<xsl:for-each select="String[Key='Password']"><xsl:value-of select="Value" /><xsl:text>&#13;&#10;</xsl:text></xsl:for-each>
</xsl:template>

</xsl:stylesheet>
